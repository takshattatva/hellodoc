-- Table: public.EncounterForm

-- DROP TABLE IF EXISTS public."EncounterForm";

CREATE TABLE IF NOT EXISTS public."Encounter"
(
"Id" serial NOT NULL PRIMARY KEY,
"RequestId" integer NOT NULL,
"isFinalized" bit(1) DEFAULT '0'::"bit",
"HistoryIllness" character varying(200) COLLATE pg_catalog."default",
"MedicalHistory" character varying(200) COLLATE pg_catalog."default",
"Date" timestamp without time zone,
"Medications" character varying(200) COLLATE pg_catalog."default",
"Allergies" character varying(200) COLLATE pg_catalog."default",
"Temp" numeric,
"HR" numeric,
"RR" numeric,
"BP(S)" integer,
"BP(D)" integer,
"O2" numeric,
"Pain" character varying(200) COLLATE pg_catalog."default",
"HEENT" character varying(200) COLLATE pg_catalog."default",
"CV" character varying(200) COLLATE pg_catalog."default",
"Chest" character varying(200) COLLATE pg_catalog."default",
"ABD" character varying(200) COLLATE pg_catalog."default",
"Extr" character varying(200) COLLATE pg_catalog."default",
"Skin" character varying(200) COLLATE pg_catalog."default",
"Neuro" character varying(200) COLLATE pg_catalog."default",
"Other" character varying(200) COLLATE pg_catalog."default",
"Diagnosis" character varying(200) COLLATE pg_catalog."default",
"TreatmentPlan" character varying(200) COLLATE pg_catalog."default",
"MedicationDispensed" character varying(200) COLLATE pg_catalog."default",
"Procedures" character varying(200) COLLATE pg_catalog."default",
"FollowUp" character varying(200) COLLATE pg_catalog."default",
CONSTRAINT fk_encounter_request FOREIGN KEY ("RequestId")
REFERENCES public."request" ("requestid") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Encounter"
OWNER to postgres;