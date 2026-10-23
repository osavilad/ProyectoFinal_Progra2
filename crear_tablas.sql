CREATE TABLE IF NOT EXISTS Propietarios (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre    TEXT NOT NULL,
    Telefono  TEXT NOT NULL,
    Email     TEXT,
    Direccion TEXT
);

CREATE TABLE IF NOT EXISTS Veterinarios (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre       TEXT NOT NULL,
    Especialidad TEXT NOT NULL,
    Colegiado    TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Mascotas (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre          TEXT NOT NULL,
    Especie         TEXT NOT NULL,
    Raza            TEXT,
    FechaNacimiento TEXT,
    Sexo            TEXT NOT NULL CHECK (Sexo IN ('M','H')),
    PropietarioId   INTEGER NOT NULL,
    FOREIGN KEY (PropietarioId) REFERENCES Propietarios(Id)
);

CREATE TABLE IF NOT EXISTS Consultas (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    MascotaId     INTEGER NOT NULL,
    VeterinarioId INTEGER NOT NULL,
    Fecha         TEXT NOT NULL,
    Motivo        TEXT NOT NULL,
    Diagnostico   TEXT,
    Tratamiento   TEXT,
    Costo         REAL NOT NULL DEFAULT 0 CHECK (Costo >= 0),
    FOREIGN KEY (MascotaId)     REFERENCES Mascotas(Id),
    FOREIGN KEY (VeterinarioId) REFERENCES Veterinarios(Id)
);

CREATE INDEX IF NOT EXISTS IX_Mascotas_PropietarioId  ON Mascotas(PropietarioId);
CREATE INDEX IF NOT EXISTS IX_Consultas_MascotaId     ON Consultas(MascotaId);
CREATE INDEX IF NOT EXISTS IX_Consultas_VeterinarioId ON Consultas(VeterinarioId);
