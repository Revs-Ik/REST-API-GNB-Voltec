# Prueba Técnica Voltec - API REST

Para esta prueba se ha elaborado una API Rest con C# y usando una base de datos SQL Server.
Tiempo aprox. dedicado por día (miércoles - sábado): 4 horas.

## Índice de Contenidos

- [0. Cómo Usar](#0-cómo-usar)
- [1. Videos Resumen](#1-videos-resumen-2-3h)
- [2. Comprender lo que se solicita](#2-comprender-lo-que-se-solicita-030h)
- [3. Elaborar un breve esquema](#3-elaborar-un-breve-esquema-015h)
- [4. Escribir código](#4-escribir-código-8h)
  - [4.1 Base de datos](#41-base-de-datos-030h)
  - [4.2 Dependencias](#42-dependencias)
- [5. Revisar y corregir](#5-revisar-y-corregir-4h)
- [6. Desplegar](#6-desplegar-5-6h)

## 0. Cómo Usar

**IMPORTANTE**: Para desplegar esta API es necesario configurar un servidor SQL y añadir un archivo `.env` con una variable `CONNECTION_STRING` que sea la cadena de conexión a este servidor.
Actualmente 21 Jul 2024, se encuentra desplegada en [https://restapignb20240720003426.azurewebsites.net/](https://restapignb20240720003426.azurewebsites.net/) y se espera que siga así hasta miércoles 24 Jul 2024.

### Endpoints

- **Ver todas las transacciones:**

```http
GET: https://restapignb20240720003426.azurewebsites.net/api/transactions/all
```

- **Añadir transacciones:**

Requiere el header `api-key` con valor "privileged API key". Además, las transacciones que se quieran añadir se pasan en el Body en forma de lista de diccionarios.

```http
POST: https://restapignb20240720003426.azurewebsites.net/api/transactions/add
```

- **Buscar transacciones por su SKU:**

Donde `t2006` representa el codigo SKU de buscado.
(Retorna todas las transacciones en EUR y la suma total de estas, también en EUR).

```http
GET: https://restapignb20240720003426.azurewebsites.net/api/transactions/search?sku=t2006
```

- **Ver todas las rates:**

```http
GET: https://restapignb20240720003426.azurewebsites.net/api/rates/all
```

- **Añadir rates:**

Requiere el header `api-key` con valor "privileged API key". Además, las transacciones que se quieran añadir se pasan en el Body en forma de lista de diccionarios.

```http
POST: https://restapignb20240720003426.azurewebsites.net/api/rates/add
```

## 1. Videos Resumen (2-3h)

Lo primero que hice antes de empezar fue ver dos videos:

- [Resumen intensivo de las estructuras en C#](https://www.youtube.com/watch?v=j8sxDnr7nPY&ab_channel=hdeleon.net) (Clases, objectos, Herencia, Interfaces, Generics, Lambda, LINQ)
- [API Rest con C#](https://www.youtube.com/watch?v=3XRLIEyKHMg&t=5457s&ab_channel=Codigo369) (Estructura básica de una API Rest en C# y dependencias)

## 2. Comprender lo que se solicita (0:30h)

Primero, leí la prueba técnica y me aseguré de entender todo lo que se solicita, además de imaginar posibles problemas que puedan surgir.

## 3. Elaborar un breve esquema (0:15h)

Elaboré un breve esquema de la estructura de esta API antes de empezar a programarla. Decidí usar MS SQL Server para la base de datos de esta API debido a su fácil integración con las plantillas de la plataforma .NET

## 4. Escribir código (8h)

Partiendo de la plantilla ASP.NET Core Web API, creé los controladores, modelos y métodos para esta API. Además, implementé un sistema de logs para registrar las peticiones que recibe, las modificaciones en el servidor SQL y los errores que surjan.

En lo que inverti más tiempo es sin duda la logica para convertir las transacciones a EUR:

En un primer momento cree un metodo recursivo para buscar una ruta para la conversion a EUR y, aunque funcionaba, no estaba seguro de que no hubieran excepciones en las que pudiera no encontrar un camino que si era existente.

Por esto, le pregunte a ChatGPT y este me ofrecio una respuesta en la que usaba un HashSet. Como la considere más eficiente y segura, esta fue la solución que se quedó en el codigo.

### 4.1 Base de datos (0:30h)

Configure una base de datos en un servidor MS SQL Server, conteniendo las tablas `transactions` y `rates` ambas con un ID automatico que no es usado por la API.

### 4.2 Dependencias

Añadí las dependencias `Microsoft.Data.SqlClient` para la conexión con el servidor de base de datos y `DotNetEnv` para proteger información sensible, como la cadena de conexión al servidor, en un archivo `.env`. No estoy completamente seguro de la necesidad de esta protección en una API, dado que en ningun momento se ofrece la posibilidad de interactuar con el servidor más alla de las peticiones prestablecidas.

## 5. Revisar y corregir (4h)

Revisé el código, realicé pequeñas reestructuraciones y probé los endpoints para identificar y corregir errores que no se habían tenido en cuenta previamente.

## 6. Desplegar (5-6h)

Dado que la prueba se valoraría el lunes y quedaba algo de tiempo libre el fin de semana, decidí probar el despliegue en un entorno real.

Inicialmente intenté desplegar la API en un servidor Raspberry Pi, pero debido a la incompatibilidad del sistema operativo, no pude instalarla directamente.

Luego, en este mismo Raspberry Pi, intenté usar un contenedor Docker con la esperanza de evadir problemas de compatibilidad del sistema, pero encontré varios problemas adicionales que no he resuelto a día de hoy (domingo 21 Jul), posiblemente relacionados con certificados que Visual Studio añade al iniciar el contenedor y faltan al iniciarlo en otro entorno.

Finalmente, decidí desplegar la API en Microsoft Azure utilizando la prueba gratuita, donde no encontré ninguna dificultad.
