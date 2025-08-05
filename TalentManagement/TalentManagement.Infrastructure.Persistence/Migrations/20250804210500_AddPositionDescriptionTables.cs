using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionDescriptionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PositionDescriptions",
                columns: table => new
                {
                    PdSeqNum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdNbr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PdEffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdCareerLadderInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdTargetGradeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdFlsaInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdFinDisclosureInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdClassifierCommentsText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdArchiveInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdOldGemsPdNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdClassifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdClassifierHruId = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrdCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    JobFunction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobFunctionEffDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdsStateCd = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GvtOccSeries = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GvtPayPlan = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GvtOrgTitleCd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GvtOrgTitleEffDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GvtPosnTitleCd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GvtPosnTitleEffDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdPositionTitleText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PdOrgTitleText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SkillCd1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PsSourceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdReplacePdNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdManagerLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdAcquisitionPosInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdLimitationsOfUse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdOriginOrgCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdthAssignedOwner = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PdFlsaUrlText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdLeoType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdBargUnitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdOldGemsJobcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdCreateId = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdCreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PdUpdateId = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PdUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PdEvalStatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdTransType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdEffectiveSeqNum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdReasonForSubmission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIaAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdClpdGradeInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIaLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CrmTicketNum = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PdInterdisciplinaryInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdExplantnComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIpop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIntro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdOriginalEffdt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Guid = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionDescriptions", x => x.PdSeqNum);
                });

            migrationBuilder.CreateTable(
                name: "PositionDescriptionAudit",
                columns: table => new
                {
                    PdAuditSeqNum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdSeqNum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdNbr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PdEffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdCareerLadderInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdTargetGradeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdFlsaInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdFinDisclosureInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdClassifierCommentsText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PdArchiveInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdOldGemsPdNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdClassifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdClassifierHruId = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrdCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    JobFunction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobFunctionEffDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdsStateCd = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GvtOccSeries = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GvtPayPlan = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GvtOrgTitleCd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GvtOrgTitleEffDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GvtPosnTitleCd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GvtPosnTitleEffDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdPositionTitleText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PdOrgTitleText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SkillCd1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PsSourceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdReplacePdNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdManagerLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdAcquisitionPosInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdLimitationsOfUse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdOriginOrgCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdthAssignedOwner = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PdFlsaUrlText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdLeoType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdBargUnitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdaCreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PdCreateId = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdUpdateId = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PdUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PdEvalStatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdTransType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdEffectiveSeqNum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdReasonForSubmission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIaAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdClpdGradeInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIaLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CrmTicketNum = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PdInterdisciplinaryInd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdExplantnComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIpop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdIntro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdOriginalEffdt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionDescriptionAudit", x => x.PdAuditSeqNum);
                    table.ForeignKey(
                        name: "FK_PositionDescriptionAudit_PositionDescriptions_PdSeqNum",
                        column: x => x.PdSeqNum,
                        principalTable: "PositionDescriptions",
                        principalColumn: "PdSeqNum",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionDescriptionAudit_PdSeqNum",
                table: "PositionDescriptionAudit",
                column: "PdSeqNum");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionDescriptionAudit");

            migrationBuilder.DropTable(
                name: "PositionDescriptions");
        }
    }
}
