Presentación
============

 

Componentes Principales
-----------------------

Si bien ya existen demasiadas bibliotecas e implementaciones propias por parte
de la misma Ms. para la API de persistencia de datos, con este proyecto se
intenta: *unificar la interacción con distintos SGBDs*, también brindar una
herramienta totalmente manipulable y adaptable por cualquier persona con
conocimientos basicos de programacion en .NET.-

 

Detalles
--------

 

La biblioteca tiene como pilares principales los siguientes elementos:

 

Una clase de utilería para cada SGBD, en la que se centralizan las operaciones
propias que se realizan contra una base de datos, las clases *\_Utiles*.-

Una clase base para definición de Estructuras de Tablas, que a su vez hace de
"VO" (o DTO) *OTDbase*, y

Una clase base para Administrar la interacción con las Tablas, la clase
*ADMbase*, que hace de "DAO".

 

Básico
------

La biblioteca como tal, en su version actual (v3.5), fue desarrollada pensando
en facilitar y unificar las tareas de acceso a los datos de los diferentes
motores de bases de datos con los que trabajamos en la empresa para la cual
trabajo, tomando como referencias los ORM's de otros lenguajes de programacion,
como Hibernate/JPA de Java y SQL Alchemy de Python.-

 

A pesar de ser 100% funcional, continúa en etapa de desarrollo de mejoras, pues
quedan por resolver algunas cuestiones como las relaciones entre tablas por
medio de los accesores sin necesidad de ejecutas las llamadas a los metodos de
los administradores de las tablas relacionadas, o el hecho de poder realizar
referencias a los campos a traves de las propiedades de las clases OTD's para
asi poder escribir sentencias tipo Linq o HQL.

