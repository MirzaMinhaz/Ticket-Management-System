using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTicketCounterIdIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        -- Step 1: Drop foreign keys referencing TicketCounters
        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tickets_TicketCounters_DepartureCounterId')
            ALTER TABLE Tickets DROP CONSTRAINT FK_Tickets_TicketCounters_DepartureCounterId;

        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tickets_TicketCounters_BookingCounterId')
            ALTER TABLE Tickets DROP CONSTRAINT FK_Tickets_TicketCounters_BookingCounterId;

        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tickets_TicketCounters_ArrivalCounterId')
            ALTER TABLE Tickets DROP CONSTRAINT FK_Tickets_TicketCounters_ArrivalCounterId;

        -- Step 2: Drop primary key
        ALTER TABLE TicketCounters DROP CONSTRAINT PK_TicketCounters;

        -- Step 3: Add new identity column
        ALTER TABLE TicketCounters ADD TempId INT IDENTITY(1,1);

        -- Step 4: Copy old Id values (if needed)
        -- Optional: UPDATE TicketCounters SET TempId = Id;

        -- Step 5: Drop old Id column
        ALTER TABLE TicketCounters DROP COLUMN Id;

        -- Step 6: Rename TempId to Id
        EXEC sp_rename 'TicketCounters.TempId', 'Id', 'COLUMN';

        -- Step 7: Recreate primary key
        ALTER TABLE TicketCounters ADD CONSTRAINT PK_TicketCounters PRIMARY KEY (Id);

        -- Step 8: Recreate foreign keys
        ALTER TABLE Tickets ADD CONSTRAINT FK_Tickets_TicketCounters_DepartureCounterId
            FOREIGN KEY (DepartureCounterId) REFERENCES TicketCounters(Id);

        ALTER TABLE Tickets ADD CONSTRAINT FK_Tickets_TicketCounters_BookingCounterId
            FOREIGN KEY (BookingCounterId) REFERENCES TicketCounters(Id);

        ALTER TABLE Tickets ADD CONSTRAINT FK_Tickets_TicketCounters_ArrivalCounterId
            FOREIGN KEY (ArrivalCounterId) REFERENCES TicketCounters(Id);
    ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
