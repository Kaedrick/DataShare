CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS "Users" (
    "Id" uuid PRIMARY KEY,
    "Email" text NOT NULL UNIQUE,
    "PasswordHash" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS "StoredFiles" (
    "Id" uuid PRIMARY KEY,
    "OriginalName" text NOT NULL,
    "StoredName" text NOT NULL,
    "ContentType" text NOT NULL,
    "Size" bigint NOT NULL,
    "RelativePath" text NOT NULL,
    "Token" text NOT NULL UNIQUE,
    "PasswordHash" text NULL,
    "UploadedAt" timestamp with time zone NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "UserId" uuid NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE
);
