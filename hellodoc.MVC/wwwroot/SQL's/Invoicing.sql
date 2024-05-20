CREATE TABLE "PayrateCategory" (
"PayrateCategoryId" SERIAL PRIMARY KEY,
"CategoryName" VARCHAR(256) NOT NULL
);


CREATE TABLE "PayrateByProvider" (
"PayrateId" SERIAL PRIMARY KEY,
"PayrateCategoryId" INT NOT NULL,
"PhysicianId" INT NOT NULL,
"Payrate" DECIMAL(8,3) NOT NULL,
"CreatedBy" VARCHAR(128) NOT NULL,
"CreatedDate" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
"ModifiedBy" VARCHAR(128),
"ModifiedDate" TIMESTAMP WITHOUT TIME ZONE,
CONSTRAINT fk_payratecategory FOREIGN KEY ("PayrateCategoryId") REFERENCES "PayrateCategory" ("PayrateCategoryId"),
CONSTRAINT fk_provider FOREIGN KEY ("PhysicianId") REFERENCES Physician ("physicianid"),
CONSTRAINT fk_createdby FOREIGN KEY ("CreatedBy") REFERENCES Aspnetusers ("id"),
CONSTRAINT fk_modifiedby FOREIGN KEY ("ModifiedBy") REFERENCES Aspnetusers ("id")
);


CREATE TABLE "Timesheet" (
"TimesheetId" SERIAL PRIMARY KEY,
"PhysicianId" INT NOT NULL,
"StartDate" DATE NOT NULL,
"EndDate" DATE NOT NULL,
"IsFinalize" BOOLEAN,
"IsApproved" BOOLEAN,
"BonusAmount" VARCHAR(128),
"AdminNotes" TEXT,
"CreatedBy" VARCHAR(128) NOT NULL,
"CreatedDate" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
"ModifiedBy" VARCHAR(128),
"ModifiedDate" TIMESTAMP WITHOUT TIME ZONE,
CONSTRAINT fk_physician FOREIGN KEY ("PhysicianId") REFERENCES Physician ("physicianid"),
CONSTRAINT fk_createdby FOREIGN KEY ("CreatedBy") REFERENCES Aspnetusers ("id"),
CONSTRAINT fk_modifiedby FOREIGN KEY ("ModifiedBy") REFERENCES Aspnetusers ("id")
);

CREATE TABLE "TimesheetDetail" (
"TimesheetDetailId" SERIAL PRIMARY KEY,
"TimesheetId" INT NOT NULL,
"TimesheetDate" DATE NOT NULL,
"TotalHours" DECIMAL,
"IsWeekend" BOOLEAN,
"NumberOfHouseCall" INT,
"NumberOfPhoneCall" INT,
"ModifiedBy" VARCHAR(128),
"ModifiedDate" TIMESTAMP WITHOUT TIME ZONE,
CONSTRAINT fk_timesheet FOREIGN KEY ("TimesheetId") REFERENCES "Timesheet" ("TimesheetId"),
CONSTRAINT fk_modifiedby FOREIGN KEY ("ModifiedBy") REFERENCES Aspnetusers ("id")
);

CREATE TABLE "TimesheetDetailReimbursement" (
"TimesheetDetailReimbursementId" SERIAL PRIMARY KEY,
"TimesheetDetailId" INT NOT NULL,
"ItemName" VARCHAR(500) NOT NULL,
"Amount" INT NOT NULL,
"Bill" VARCHAR(500) NOT NULL,
"IsDeleted" BOOLEAN,
"CreatedBy" VARCHAR(128) NOT NULL,
"CreatedDate" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
"ModifiedBy" VARCHAR(128),
"ModifiedDate" TIMESTAMP WITHOUT TIME ZONE,
CONSTRAINT fk_timesheetdetail FOREIGN KEY ("TimesheetDetailId") REFERENCES "TimesheetDetail" ("TimesheetDetailId"),
CONSTRAINT fk_createdby FOREIGN KEY ("CreatedBy") REFERENCES Aspnetusers ("id"),
CONSTRAINT fk_modifiedby FOREIGN KEY ("ModifiedBy") REFERENCES Aspnetusers ("id")
);


