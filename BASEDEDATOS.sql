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

select * from users where RoleID = 1;

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
    
    --  Procedimientos. Del cliente deben de estar del lado de la DB
    
    
    -- GET ESTUDIANTE  Obtener lista de estudiantes
DELIMITER $$

CREATE PROCEDURE GetEstudiantes()
BEGIN
    SELECT u.UserID, s.NoControl, s.Nombre
    FROM Users u
    INNER JOIN Students s ON u.UserID = s.UserID;
END $$

DELIMITER ;

DELIMITER $$

-- 2
CREATE PROCEDURE GetUltimaRespuestaAnsiedad(IN userId INT)
BEGIN
    SELECT *
    FROM Responses
    WHERE UserID = userId AND QuestionnaireID = 'GAD7'
    ORDER BY TakenAt DESC
    LIMIT 1;
END $$

DELIMITER ;


-- 3

DELIMITER $$

CREATE PROCEDURE GetHistorialRespuestas(IN userId INT)
BEGIN
    SELECT *
    FROM Responses
    WHERE UserID = userId
    ORDER BY TakenAt;
END $$

DELIMITER ;

-- 4

DELIMITER $$

CREATE PROCEDURE GetPromediosPorUsuario()
BEGIN
    SELECT UserID, QuestionnaireID, AVG(TotalScore) AS Prom
    FROM Responses
    GROUP BY UserID, QuestionnaireID;
END $$

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE GetUltimaRespuestaEstres(IN userId INT)
BEGIN
    SELECT *
    FROM Responses
    WHERE UserID = userId AND QuestionnaireID = 'PSS14'
    ORDER BY TakenAt DESC
    LIMIT 1;
END $$

DELIMITER ;

SHOW PROCEDURE STATUS WHERE Db = 'cuestionarios_psi';



DELIMITER $$

DELIMITER $$
-- DROP PROCEDURE IF EXISTS LoginUsuario;
CREATE PROCEDURE LoginUsuario(IN p_email VARCHAR(120))
BEGIN
    SELECT  u.UserID,
            u.Email,
            u.PasswordHash,
            u.RoleID,
            u.CreatedAt          -- ←  columna que faltaba
    FROM    Users u
    WHERE   u.Email = p_email;
END $$
DELIMITER ;


DELIMITER $$

CREATE PROCEDURE RegistrarUsuario(
    IN p_email VARCHAR(120),
    IN p_passwordHash CHAR(60),
    IN p_roleID TINYINT
)
BEGIN
    INSERT INTO Users (Email, PasswordHash, RoleID)
    VALUES (p_email, p_passwordHash, p_roleID);
    
    -- Devuelve el ID del nuevo usuario
    SELECT LAST_INSERT_ID() AS UserID;
END $$

DELIMITER ;


DELIMITER $$

CREATE PROCEDURE RegistrarEstudiante(
    IN p_userID INT,
    IN p_nombre VARCHAR(80),
    IN p_edad TINYINT,
    IN p_sexo VARCHAR(20),
    IN p_municipio VARCHAR(80),
    IN p_carrera VARCHAR(100),
    IN p_noControl INT,
    IN p_semestre TINYINT
)
BEGIN
    INSERT INTO Students (
        UserID, Nombre, Edad, Sexo, Municipio, Carrera, NoControl, Semestre
    ) VALUES (
        p_userID, p_nombre, p_edad, p_sexo, p_municipio, p_carrera, p_noControl, p_semestre
    );
END $$

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE InsertarRespuestaGeneral(
    IN p_userID INT,
    IN p_questionnaireID CHAR(6),
    IN p_takenAt DATETIME
)
BEGIN
    INSERT INTO Responses (UserID, QuestionnaireID, TakenAt)
    VALUES (p_userID, p_questionnaireID, p_takenAt);
    
    SELECT LAST_INSERT_ID() AS ResponseID;
END $$

DELIMITER ;



DELIMITER $$

CREATE PROCEDURE InsertarDetalleRespuesta(
    IN p_responseID BIGINT,
    IN p_questionID INT,
    IN p_questionnaireID CHAR(6),
    IN p_optionID TINYINT
)
BEGIN
    INSERT INTO ResponseDetails (ResponseID, QuestionID, QuestionnaireID, OptionID)
    VALUES (p_responseID, p_questionID, p_questionnaireID, p_optionID);
END $$

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE ActualizarInterpretacionTotal(
    IN p_responseID BIGINT,
    IN p_totalScore TINYINT,
    IN p_interpretation VARCHAR(30)
)
BEGIN
    UPDATE Responses
    SET TotalScore = p_totalScore,
        Interpretation = p_interpretation
    WHERE ResponseID = p_responseID;
END $$

DELIMITER ;

-- Preguntas -------------------------------------------------------------
DELIMITER $$
CREATE PROCEDURE GetPreguntasPorCuestionario(IN p_q CHAR(6))
BEGIN
    SELECT QuestionID, QuestionnaireID, NumberInForm, Text, IsReverse
    FROM Questions
    WHERE QuestionnaireID = p_q
    ORDER BY NumberInForm;
END $$
DELIMITER ;
-- Opciones --------------------------------------------------------------
DELIMITER $$
CREATE PROCEDURE GetOpcionesPorCuestionario(IN p_q CHAR(6))
BEGIN
    SELECT QuestionnaireID, OptionID, Text, Score
    FROM Options
    WHERE QuestionnaireID = p_q
    ORDER BY OptionID;
END $$
DELIMITER ;
-- Última respuesta genérica --------------------------------------------
DELIMITER $$
CREATE PROCEDURE GetUltimaRespuesta(
    IN p_userID INT,
    IN p_questionnaireID CHAR(6)
)
BEGIN
    SELECT *
    FROM Responses
    WHERE UserID = p_userID AND QuestionnaireID = p_questionnaireID
    ORDER BY TakenAt DESC
    LIMIT 1;
END $$
DELIMITER ;


-- ************************************************************************
USE cuestionarios_psi;
DELIMITER $$

-- DROP PROCEDURE IF EXISTS GetPreguntasPorCuestionario$$
CREATE PROCEDURE GetPreguntasPorCuestionario(IN p_q CHAR(6))
BEGIN
    /*  ↳ el alias de columna DEBE coincidir con la propiedad de la clase Question  */
    SELECT  QuestionID,
            QuestionnaireID,
            NumberInForm,
            Text,
            IsReverse           -- ← bool
    FROM Questions
    WHERE QuestionnaireID = p_q
    ORDER BY NumberInForm;
END$$

-- DROP PROCEDURE IF EXISTS GetOpcionesPorCuestionario$$
CREATE PROCEDURE GetOpcionesPorCuestionario(IN p_q CHAR(6))
BEGIN
    SELECT  QuestionnaireID,
            OptionID,
            Text,
            Score
    FROM Options
    WHERE QuestionnaireID = p_q
    ORDER BY OptionID;
END$$

-- DROP PROCEDURE IF EXISTS GetUltimaRespuesta$$
CREATE PROCEDURE GetUltimaRespuesta(IN p_userID INT, IN p_questionnaireID CHAR(6))
BEGIN
    SELECT *
    FROM Responses
    WHERE UserID = p_userID AND QuestionnaireID = p_questionnaireID
    ORDER BY TakenAt DESC
    LIMIT 1;
END$$

-- DROP PROCEDURE IF EXISTS InsertarRespuestaGeneral$$
CREATE PROCEDURE InsertarRespuestaGeneral(IN p_userID INT,
                                          IN p_questionnaireID CHAR(6),
                                          IN p_takenAt DATETIME)
BEGIN
    INSERT INTO Responses(UserID, QuestionnaireID, TakenAt)
    VALUES (p_userID, p_questionnaireID, p_takenAt);
    SELECT LAST_INSERT_ID() AS ResponseID;
END$$

-- DROP PROCEDURE IF EXISTS InsertarDetalleRespuesta$$
CREATE PROCEDURE InsertarDetalleRespuesta(IN p_responseID BIGINT,
                                          IN p_questionID INT,
                                          IN p_questionnaireID CHAR(6),
                                          IN p_optionID TINYINT)
BEGIN
    INSERT INTO ResponseDetails(ResponseID, QuestionID, QuestionnaireID, OptionID)
    VALUES (p_responseID, p_questionID, p_questionnaireID, p_optionID);
END$$

-- DROP PROCEDURE IF EXISTS ActualizarInterpretacionTotal$$
CREATE PROCEDURE ActualizarInterpretacionTotal(IN p_responseID BIGINT,
                                               IN p_totalScore TINYINT,
                                               IN p_interpretation VARCHAR(30))
BEGIN
    UPDATE Responses
    SET TotalScore    = p_totalScore,
        Interpretation = p_interpretation
    WHERE ResponseID  = p_responseID;
END$$
DELIMITER ;


-- **************************************


USE cuestionarios_psi;
ALTER TABLE Questionnaires 
    ADD COLUMN Description TEXT AFTER MaxScore;

-- -- Rellena las descripciones largas
UPDATE Questionnaires SET Description = 'El GAD-7 es un instrumento auto-aplicable de 7 ítems que evalúa el trastorno de ansiedad generalizada durante las últimas 2 semanas (DSM-5). Cada ítem se puntúa de 0 (“nada”) a 3 (“casi todos los días”). La puntuación total oscila entre 0 y 21; una puntuación ≥ 10 indica ansiedad clínicamente relevante. Uso académico: muy útil para investigar niveles de ansiedad en poblaciones generales y estudiantiles.'

WHERE QuestionnaireID='GAD7';

UPDATE Questionnaires SET Description = 'Escala de Estrés Percibido-14 (PSS-14, Cohen et al., 1983). Mide el nivel de estrés percibido durante el último mes. Consta de 14 ítems con respuestas tipo Likert de 0 (“nunca”) a 4 (“muy a menudo”). Estudios en español muestran adecuada validez y confiabilidad (Campos-Aria et al., 2009; González & Landero, 2007). Se ha comprobado una estructura bifactorial y coeficientes ω de .84 – .81. Muy utilizada en investigación latinoamericana para estimar estrés percibido.'

WHERE QuestionnaireID='PSS14';

-- -- SP para leer la descripción
DROP   PROCEDURE IF EXISTS GetDescripcionCuestionario;
DELIMITER $$
CREATE PROCEDURE GetDescripcionCuestionario(IN p_q CHAR(6))
BEGIN
    SELECT Description FROM Questionnaires WHERE QuestionnaireID=p_q LIMIT 1;
END$$
DELIMITER ;



/* ----------------------------------------------------------- */
/*  A)  Última respuesta de ESTRÉS  (si no la tenías).          */
/* ----------------------------------------------------------- */
DROP PROCEDURE IF EXISTS GetUltimaRespuestaEstres;
DELIMITER $$
CREATE PROCEDURE GetUltimaRespuestaEstres(IN p_userID INT)
BEGIN
    SELECT *
    FROM   Responses
    WHERE  UserID = p_userID
      AND  QuestionnaireID = 'PSS14'
    ORDER BY TakenAt DESC
    LIMIT 1;
END$$
DELIMITER ;

/* ----------------------------------------------------------- */
/*  B)  Resumen “dashboard” de cada estudiante ✓                */
/* ----------------------------------------------------------- */
DROP PROCEDURE IF EXISTS GetResumenEstudiantes;
DELIMITER $$
CREATE PROCEDURE GetResumenEstudiantes()
BEGIN
    /*  Devuelve: UserID, NoControl, Nombre,
                  UltGAD_Score, UltGAD_Taken,
                  UltPSS_Score, UltPSS_Taken         */
    SELECT  s.UserID,
            s.NoControl,
            s.Nombre,

            ( SELECT  r.TotalScore
              FROM    Responses r
              WHERE   r.UserID = s.UserID
                AND   r.QuestionnaireID = 'GAD7'
              ORDER BY r.TakenAt DESC
              LIMIT 1 )             AS UltGAD_Score,

            ( SELECT  r.TakenAt
              FROM    Responses r
              WHERE   r.UserID = s.UserID
                AND   r.QuestionnaireID = 'GAD7'
              ORDER BY r.TakenAt DESC
              LIMIT 1 )             AS UltGAD_Taken,

            ( SELECT  r.TotalScore
              FROM    Responses r
              WHERE   r.UserID = s.UserID
                AND   r.QuestionnaireID = 'PSS14'
              ORDER BY r.TakenAt DESC
              LIMIT 1 )             AS UltPSS_Score,

            ( SELECT  r.TakenAt
              FROM    Responses r
              WHERE   r.UserID = s.UserID
                AND   r.QuestionnaireID = 'PSS14'
              ORDER BY r.TakenAt DESC
              LIMIT 1 )             AS UltPSS_Taken
    FROM Students s
    ORDER BY s.Nombre;
END$$
DELIMITER ;

