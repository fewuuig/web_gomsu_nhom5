-- V2__Add_TinNhanChat_Table.sql
-- Bảng lưu trữ tin nhắn chat tư vấn trực tuyến gốm sứ Bát Tràng

IF OBJECT_ID('TinNhanChat', 'U') IS NULL
BEGIN
    CREATE TABLE TinNhanChat (
        Id          BIGINT IDENTITY(1,1)    NOT NULL,
        SessionId   VARCHAR(50)             NOT NULL,
        SenderId    NVARCHAR(50)            NOT NULL,
        SenderName  NVARCHAR(100)           NOT NULL,
        Content     NVARCHAR(MAX)           NOT NULL,
        IsNhanVien  BIT                     NOT NULL DEFAULT 0,
        ThoiGian    DATETIME2               NOT NULL DEFAULT GETDATE(),

        CONSTRAINT PK_TinNhanChat PRIMARY KEY CLUSTERED (Id)
    );

    CREATE NONCLUSTERED INDEX IX_TinNhanChat_SessionId_Id
    ON TinNhanChat (SessionId, Id DESC);
END
GO
