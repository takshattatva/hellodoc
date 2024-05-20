CREATE TABLE IF NOT EXISTS public."ChatHistory"
(
    "Id" integer NOT NULL DEFAULT nextval('"ChatHistory_Id_seq"'::regclass),
    "RequestId" integer,
    "SenderAspId" character varying(255) COLLATE pg_catalog."default",
    "ReceiverAspId" character varying(255) COLLATE pg_catalog."default",
    "Time" time without time zone,
    "Message" character varying(255) COLLATE pg_catalog."default",
    "CreatedDate" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "ChatHistory_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "ChatHistory_RequestId_fkey" FOREIGN KEY ("RequestId")
        REFERENCES public.request (requestid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

CREATE TABLE IF NOT EXISTS public."UserConnection"
(
    "Id" integer NOT NULL DEFAULT nextval('"UserConnection_Id_seq"'::regclass),
    "ConnectionId" character varying(255) COLLATE pg_catalog."default",
    "UserId" character varying(255) COLLATE pg_catalog."default",
    "RequestId" character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT "UserConnection_pkey" PRIMARY KEY ("Id")
)
