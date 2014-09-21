using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Data
{
    public class DbInitializer : IDatabaseInitializer<ServerDataContext>
    {
        DbTransaction transaction;
        DbCommand command;

        public void InitializeDatabase(ServerDataContext context)
        {
            var conn = context.Database.Connection;
            conn.Open();
            try
            {
                using (transaction = conn.BeginTransaction())
                {
                    // init command
                    command = conn.CreateCommand();
                    command.Transaction = transaction;

                    // update
                    int version = CheckVersion();
                    string script;
                    while((script = GetUpdateScript(version + 1)) != null)
                    {
                        version++;
                        command.CommandText = script;
                        command.ExecuteNonQuery();
                    }

                    // confirm version and commit
                    SetVersion(version);
                    transaction.Commit();
                }
            }
            finally
            {
                transaction = null;
            }
            conn.Close();
        }

        private int CheckVersion()
        {
            command.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = \"table\" and name = \"DbVersion\"";
            var versionTableExists = (Int32)Convert.ChangeType(command.ExecuteScalar(), typeof(Int32));

            int result = 0;

            if (versionTableExists == 0)
            {
                command.CommandText = 
                    @"CREATE TABLE `DbVersion` (
		                `Id`	INTEGER NOT NULL CHECK(Id = 1) UNIQUE,
		                `Version`	INTEGER NOT NULL,
		                PRIMARY KEY(Id)
	                ); 
                    INSERT INTO `DbVersion` (`Id`,`Version`) VALUES (1, 0);";
                command.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = "SELECT `Version` FROM `DbVersion` WHERE `Id` = 1";
                result = (Int32)Convert.ChangeType(command.ExecuteScalar(), typeof(Int32));
            }

            return result;
        }

        private void SetVersion(int version)
        {
            command.CommandText = "UPDATE `DbVersion` SET `Version` = @version WHERE `Id` = 1";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "version";
            parameter.Value = version;
            command.Parameters.Add(parameter);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }

        public string GetUpdateScript(int version)
        {
            // Datatypes: http://www.sqlite.org/datatype3.html

            switch (version)
            {
                case 1:
                    return @"

CREATE TABLE `Repository` (
	`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`UrlName`	TEXT NOT NULL UNIQUE,
	`SourceFolder`	TEXT NOT NULL,
	`Command`	TEXT,
	`CommandArgs`	TEXT
);

CREATE TABLE `Client` (
	`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`CertificateHash`	BLOB,
	`Name`	TEXT
);

CREATE TABLE `PendingCertificate` (
	`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`CertificateHash`	BLOB NOT NULL,
	`LastAttemptUtc`	INTEGER NOT NULL,
    `IpAddress`         TEXT NOT NULL
);

CREATE TABLE `ClientGroup` (
    `Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    `Name`  TEXT NOT NULL UNIQUE
);

CREATE TABLE `RepositoryToClientGroup` (
    `RepositoryId`	INTEGER NOT NULL UNIQUE,
    `ClientGroupId`	INTEGER NOT NULL UNIQUE,
    PRIMARY KEY (RepositoryId, ClientGroupId),
    FOREIGN KEY (RepositoryId) REFERENCES Client(Id),
    FOREIGN KEY (ClientGroupId) REFERENCES ClientGroup(Id)
);

CREATE TABLE `ClientToClientGroup` (
    `ClientId`	INTEGER NOT NULL UNIQUE,
    `ClientGroupId`	INTEGER NOT NULL UNIQUE,
    PRIMARY KEY (ClientId, ClientGroupId),
    FOREIGN KEY (ClientId) REFERENCES Client(Id),
    FOREIGN KEY (ClientGroupId) REFERENCES ClientGroup(Id)
);

";
                default:
                    return null; // null = up to date (use empty string to create empty script if needed)
            }
        }
    }
}
