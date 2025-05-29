-- ############################################################
--  ESQUEMA:   cuestionarios_psi
-- ############################################################
DROP DATABASE IF EXISTS cuestionarios_psi;
CREATE DATABASE cuestionarios_psi
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
USE cuestionarios_psi;

-- ---------- 1.  Seguridad y cuentas -------------------------
CREATE TABLE Roles (
    RoleID   TINYINT PRIMARY KEY,
    RoleName VARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserID        INT AUTO_INCREMENT PRIMARY KEY,
    Email         VARCHAR(120) NOT NULL UNIQUE,
    PasswordHash  CHAR(60)     NOT NULL,   -- BCrypt
    RoleID        TINYINT      NOT NULL,
    CreatedAt     DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

DROP TABLE IF EXISTS Students;

CREATE TABLE Students (
    UserID     INT PRIMARY KEY,
    Nombre     VARCHAR(80),
    Edad       TINYINT,
    Sexo       VARCHAR(20),
    Municipio  VARCHAR(80),
    Carrera    VARCHAR(100),
    NoControl  INT,
    Semestre   TINYINT,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);


-- ---------- 2.  Cuestionarios y preguntas -------------------
CREATE TABLE Questionnaires (
    QuestionnaireID CHAR(6) PRIMARY KEY,   -- GAD7 / PSS14
    Name            VARCHAR(60),
    MaxScore        TINYINT
);

-- Cada cuestionario puede tener su propio texto por opción
CREATE TABLE Options (
    QuestionnaireID CHAR(6),
    OptionID        TINYINT,               -- 0‑4
    Text            VARCHAR(40),
    Score           TINYINT,
    PRIMARY KEY (QuestionnaireID, OptionID),
    FOREIGN KEY (QuestionnaireID) REFERENCES Questionnaires(QuestionnaireID)
);

CREATE TABLE Questions (
    QuestionID      INT AUTO_INCREMENT PRIMARY KEY,
    QuestionnaireID CHAR(6),
    NumberInForm    TINYINT,
    Text            TEXT,
    IsReverse       BOOLEAN DEFAULT 0,
    FOREIGN KEY (QuestionnaireID) REFERENCES Questionnaires(QuestionnaireID)
);

CREATE TABLE Responses (
    ResponseID      BIGINT AUTO_INCREMENT PRIMARY KEY,
    UserID          INT,
    QuestionnaireID CHAR(6),
    TakenAt         DATETIME DEFAULT CURRENT_TIMESTAMP,
    TotalScore      TINYINT,
    Interpretation  VARCHAR(30),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (QuestionnaireID) REFERENCES Questionnaires(QuestionnaireID)
);

-- ---------- 2.b  Detalle de respuestas (arreglo de FK) ------
CREATE TABLE ResponseDetails (
    ResponseID       BIGINT,
    QuestionID       INT,
    QuestionnaireID  CHAR(6),   -- necesario para la FK compuesta
    OptionID         TINYINT,
    PRIMARY KEY (ResponseID, QuestionID),

    FOREIGN KEY (ResponseID)                  REFERENCES Responses(ResponseID),
    FOREIGN KEY (QuestionID)                  REFERENCES Questions(QuestionID),
    FOREIGN KEY (QuestionnaireID, OptionID)   REFERENCES Options(QuestionnaireID, OptionID)
);

-- ---------- 3.  Seeds --------------------------------------
-- 3.1 Roles y cuenta admin demo (pwd: admin123)
INSERT INTO Roles VALUES (1,'ADMIN'),(2,'STUDENT');
INSERT INTO Users (Email,PasswordHash,RoleID) VALUES
('admin@demo.com',
 '$2a$12$9mJiJL1xYVpoV2GHAHF9Meed9DYBz/4x39eVBIX7ZLmo3ybdNtJ5i', 1);

-- 3.2 Cuestionarios
INSERT INTO Questionnaires VALUES
('GAD7','Escala de Ansiedad Generalizada‑7',21),
('PSS14','Escala de Estrés Percibido‑14',56);

-- 3.3 Opciones
-- GAD‑7 (0‑3)
INSERT INTO Options VALUES
('GAD7',0,'Nunca',0),
('GAD7',1,'Varios días',1),
('GAD7',2,'La mitad de los días',2),
('GAD7',3,'Casi cada día',3);

-- PSS‑14 (0‑4)
INSERT INTO Options VALUES
('PSS14',0,'Nunca',0),
('PSS14',1,'Casi nunca',1),
('PSS14',2,'De vez en cuando',2),
('PSS14',3,'A menudo',3),
('PSS14',4,'Muy a menudo',4);

-- 3.4 Preguntas GAD‑7 (1‑7)
INSERT INTO Questions (QuestionnaireID,NumberInForm,Text) VALUES
('GAD7',1,'Sentirse nervioso, ansioso, notar que se le ponen los nervios de punta.'),
('GAD7',2,'No ser capaz de parar o controlar sus preocupaciones.'),
('GAD7',3,'Preocuparse demasiado sobre diferentes cosas.'),
('GAD7',4,'Dificultad para relajarse.'),
('GAD7',5,'Estar tan desasosegado que le resulta difícil parar quieto.'),
('GAD7',6,'Sentirse fácilmente disgustado o irritable.'),
('GAD7',7,'Sentirse asustado como si algo horrible pudiese pasar.');

-- 3.5 Preguntas PSS‑14 (1‑14) + bandera IsReverse
INSERT INTO Questions (QuestionnaireID,NumberInForm,Text,IsReverse) VALUES
('PSS14',1 ,'En el último mes, ¿con qué frecuencia te has sentido afectado por algo que ocurrió inesperadamente?',0),
('PSS14',2 ,'En el último mes, ¿con qué frecuencia te has sentido incapaz de controlar las cosas importantes en tu vida?',0),
('PSS14',3 ,'En el último mes, ¿con qué frecuencia te has sentido nervioso o estresado?',0),
('PSS14',4 ,'En el último mes, ¿con qué frecuencia has manejado con éxito los pequeños problemas irritantes de la vida?',1),
('PSS14',5 ,'En el último mes, ¿con qué frecuencia has sentido que has afrontado efectivamente los cambios importantes que han estado ocurriendo en tu vida?',1),
('PSS14',6 ,'En el último mes, ¿con qué frecuencia has estado seguro sobre tu capacidad para manejar tus problemas personales?',1),
('PSS14',7 ,'En el último mes, ¿con qué frecuencia has sentido que las cosas van bien?',1),
('PSS14',8 ,'En el último mes, ¿con qué frecuencia has sentido que no podías afrontar todas las cosas que tenías que hacer?',0),
('PSS14',9 ,'En el último mes, ¿con qué frecuencia has podido controlar las dificultades de tu vida?',1),
('PSS14',10,'En el último mes, ¿con qué frecuencia has sentido que tenías todo bajo control?',1),
('PSS14',11,'En el último mes, ¿con qué frecuencia has estado enfadado porque las cosas que te han ocurrido estaban fuera de tu control?',0),
('PSS14',12,'En el último mes, ¿con qué frecuencia has pensado sobre las cosas que te faltan por hacer?',0),
('PSS14',13,'En el último mes, ¿con qué frecuencia has podido controlar la forma de pasar el tiempo?',1),
('PSS14',14,'En el último mes, ¿con qué frecuencia has sentido que las dificultades se acumulan tanto que no puedes superarlas?',0);

SELECT * FROM Options;
select * from Users;
select * from students;
select * from ResponseDetails;

use cuestionarios_psi;
SELECT
    u.UserID,
    s.Nombre,
    q.Text AS Pregunta,
    o.Text AS Respuesta
FROM
    ResponseDetails rd
JOIN Responses r ON rd.ResponseID = r.ResponseID
JOIN Users u ON r.UserID = u.UserID
JOIN Students s ON s.UserID = u.UserID
JOIN Questions q ON rd.QuestionID = q.QuestionID
JOIN Options o ON rd.QuestionnaireID = o.QuestionnaireID AND rd.OptionID = o.OptionID
ORDER BY
    u.UserID, rd.ResponseID, q.NumberInForm;
