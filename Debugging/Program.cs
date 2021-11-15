using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

connection.Execute(@"
    CREATE TABLE world (
        id INTEGER NOT NULL PRIMARY KEY,
        randomNumber INTEGER NOT NULL
    );
    CREATE TABLE fortune (
        id INTEGER NOT NULL PRIMARY KEY,
        message TEXT NOT NULL
    );
    INSERT INTO fortune (message)
    VALUES
        ('fortune: No such file or directory'),
        ('A computer scientist is someone who fixes things that aren''t broken.'),
        ('After enough decimal places, nobody gives a damn.'),
        ('A bad random number generator: 1, 1, 1, 1, 1, 4.33e+67, 1, 1, 1'),
        ('A computer program does what you tell it to do, not what you want it to do.'),
        ('Emacs is a nice operating system, but I prefer UNIX. — Tom Christaensen'),
        ('Any program that runs right is obsolete.'),
        ('A list is only as strong as its weakest link. — Donald Knuth'),
        ('Feature: A bug with seniority.'),
        ('Computers make very fast, very accurate mistakes.'),
        ('<script>alert(""This should not be displayed in a browser alert box."");</script>'),
        ('フレームワークのベンチマーク');
");

var random = new Random();
for (var x = 0; x < 10000; x++)
{
    connection.Execute(
        "INSERT INTO world (randomNumber) VALUES (@Value)",
        new
        {
            Value = random.Next(1, 10001)
        });
}

var dataSet = new DataSet();
using (var reader = connection.ExecuteReader("SELECT * FROM world;SELECT * FROM fortune"))
{
    dataSet.Load(reader, LoadOption.OverwriteChanges, "world", "fortune");
}

_ = dataSet;
_ = connection;
Debugger.Break();