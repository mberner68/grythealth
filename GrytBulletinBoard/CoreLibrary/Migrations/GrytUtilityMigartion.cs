using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using CoreLibrary.Databases;

namespace CoreLibrary.Migrations
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class MigrationComposer : ComponentComposer<MigrationComponent>
    {
    }
    public class MigrationComponent : IComponent
    {
        private IScopeProvider _scopeProvider;
        private IMigrationBuilder _migrationBuilder;
        private IKeyValueService _keyValueService;
        private ILogger _logger;

        public MigrationComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }

        public void Initialize()
        {
            // Create a migration plan for a specific project/feature
            // We can then track that latest migration state/step for this project/feature
            var migrationPlan = new MigrationPlan("Board");

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan.From(string.Empty)
                .To<AddBoardTable>("BoardComponent-db")
                .To<AddNoteTable>("Add Note Table")
                .To<AddJsonString>("Add Json String")
                .To<AddCreatorString>("Add creatot String");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }
    public class AddCreatorString : MigrationBase
    {
        public AddCreatorString(IMigrationContext context) : base(context) { }

        public override void Migrate()
        {
            Logger.Debug<AddCreatorString>("Running migration {MigrationStep}", "AddCreatorString");

            if (!ColumnExists("Notes", "Creator"))
            {
                Create.Column("Creator").OnTable("Notes").AsString().Nullable().Do();

            }
            else
            {
                Logger.Debug<AddCreatorString>("Additional columns (checked for Creator) already exist, skipping migration");
            }
        }


    }
    public class AddJsonString : MigrationBase
    {
        public AddJsonString(IMigrationContext context) : base(context) { }

        public override void Migrate()
        {
            Logger.Debug<AddJsonString>("Running migration {MigrationStep}", "AddJsonString");

            if (!ColumnExists("Notes", "JsonString"))
            {
                Create.Column("JsonString").OnTable("Notes").AsString().Nullable().Do();

            }
            else
            {
                Logger.Debug<AddJsonString>("Additional columns (checked for JsonString) already exist, skipping migration");
            }
        }


    }
    public class AddBoardTable : MigrationBase
    {
        public AddBoardTable(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate()
        {
            Logger.Debug<AddBoardTable>("Running migration {MigrationStep}", "AddBoardTable");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("Boards") == false)
            {
                Create.Table<BulletinBoard>().Do();
            }
            else
            {
                Logger.Debug<AddBoardTable>("The database table {DbTable} already exists, skipping", "Boards");
            }
        }

    }
    public class AddNoteTable : MigrationBase
    {
        public AddNoteTable(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate()
        {
            Logger.Debug<AddNoteTable>("Running migration {MigrationStep}", "AddNoteTable");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("Notes") == false)
            {
                Create.Table<NoteTable>().Do();
            }
            else
            {
                Logger.Debug<AddNoteTable>("The database table {DbTable} already exists, skipping", "Notes");
            }
        }

    }
}
