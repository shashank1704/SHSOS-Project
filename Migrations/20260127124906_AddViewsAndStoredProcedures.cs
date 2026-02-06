using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHSOS.Migrations
{
    /// <inheritdoc />
    public partial class AddViewsAndStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // === VIEWS ===
            migrationBuilder.Sql(@"
                CREATE VIEW snot.vw_EnergyConsumption AS
                SELECT 
                    ec.EnergyConsumptionID, ec.DepartmentID, d.DepartmentName,
                    ec.ConsumptionDate, ec.ReadingTime, ec.MeterReadingStart,
                    ec.UnitsConsumedkWh, ec.UnitCost, ec.UsageCategory,
                    ec.PeakHourFlag, ec.TotalCost, ec.CarbonEmissionsKg, ec.RecordedAt
                FROM snot.EnergyConsumption ec
                JOIN snot.Departments d ON ec.DepartmentID = d.DepartmentID;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW snot.vw_WaterConsumption AS
                SELECT 
                    wc.ConsumptionID, wc.DepartmentID, d.DepartmentName,
                    wc.ConsumptionDate, wc.ReadingTime, wc.ReadingEnd,
                    wc.UnitsConsumedLiters, wc.UnitCost, wc.LeakageDetected,
                    wc.WeatherCategory, wc.WeatherCondition, wc.Remarks, wc.RecordedAt
                FROM snot.WaterConsumption wc
                JOIN snot.Departments d ON wc.DepartmentID = d.DepartmentID;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW snot.vw_WasteManagement AS
                SELECT 
                    wm.WasteRecordID, wm.DepartmentID, d.DepartmentName,
                    wm.WasteType, wm.WasteCategory, wm.WasteWeight,
                    wm.SegregationStatus, wm.DisposalMethod, wm.DisposalCost,
                    wm.DisinfectionCost, wm.ComplianceStatus, wm.CollectionDate, wm.RecordedAt
                FROM snot.WasteManagement wm
                JOIN snot.Departments d ON wm.DepartmentID = d.DepartmentID;
            ");

            // === STORED PROCEDURES ===
            migrationBuilder.Sql(@"
                CREATE PROCEDURE snot.usp_UpdateEnergy
                    @EnergyConsumptionID INT,
                    @UnitsConsumedkWh DECIMAL(18,2),
                    @ConsumptionDate DATETIME,
                    @UsageCategory NVARCHAR(50),
                    @PeakHourFlag BIT
                AS
                BEGIN
                    IF @UnitsConsumedkWh < 0
                        THROW 50000, 'Consumption cannot be negative.', 1;

                    UPDATE snot.EnergyConsumption
                    SET UnitsConsumedkWh = @UnitsConsumedkWh,
                        ConsumptionDate = @ConsumptionDate,
                        UsageCategory = @UsageCategory,
                        PeakHourFlag = @PeakHourFlag,
                        TotalCost = @UnitsConsumedkWh * UnitCost,
                        CarbonEmissionsKg = @UnitsConsumedkWh * 0.5
                    WHERE EnergyConsumptionID = @EnergyConsumptionID;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE snot.usp_UpdateWater
                    @ConsumptionID INT,
                    @UnitsConsumedLiters DECIMAL(18,2),
                    @ConsumptionDate DATETIME,
                    @LeakageDetected BIT,
                    @Remarks NVARCHAR(500)
                AS
                BEGIN
                    IF @UnitsConsumedLiters < 0
                        THROW 50000, 'Consumption cannot be negative.', 1;

                    UPDATE snot.WaterConsumption
                    SET UnitsConsumedLiters = @UnitsConsumedLiters,
                        ConsumptionDate = @ConsumptionDate,
                        LeakageDetected = @LeakageDetected,
                        Remarks = @Remarks
                    WHERE ConsumptionID = @ConsumptionID;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE snot.usp_UpdateWaste
                    @WasteRecordID INT,
                    @WasteType NVARCHAR(50),
                    @WasteWeight DECIMAL(18,2),
                    @CollectionDate DATETIME,
                    @ComplianceStatus NVARCHAR(50)
                AS
                BEGIN
                    IF @WasteWeight < 0
                        THROW 50000, 'Weight cannot be negative.', 1;

                    UPDATE snot.WasteManagement
                    SET WasteType = @WasteType,
                        WasteWeight = @WasteWeight,
                        CollectionDate = @CollectionDate,
                        ComplianceStatus = @ComplianceStatus
                    WHERE WasteRecordID = @WasteRecordID;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS snot.usp_UpdateWaste;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS snot.usp_UpdateWater;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS snot.usp_UpdateEnergy;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS snot.vw_WasteManagement;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS snot.vw_WaterConsumption;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS snot.vw_EnergyConsumption;");
        }
    }
}
