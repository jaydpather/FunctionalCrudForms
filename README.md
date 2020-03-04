# FunctionalCrudForms

This  is a demo app of CRUD forms in F#, a functional langauge. The user can create, read, update, delete, and search for employees.

## Projects and Directories

This app consists of several projects:
  * __/node__ folder
    * NextJS application
    * used to load UI, but not data
    * uses React with either client-side or server-side rendering
    * references JS files produced by Fable project
    * to run: 
      * cd node
      * npm run dev
  * __/fable__ folder
    * Fable is a tool that transpiles F# into JS
    * these JS files can be  referenced by the front end
    * __/client__ folder
      * contains code that is only run on the client-side
    * __/shared__ folder
      * contains code that is run on both client and server
      * example: validation logic
      * client runs the transpiled JS form of the F# code in the browser
      * microservices reference the compiled F# DLL's
  * __/backend__ folder: 
    * back end web service
    * used to load or save data, but not for loading the UI
    * F#, .Net Core
    * uses RabbitMQ to call microservices
  * __/microservices__ folder:
    * microservices that run on the back end
    * F#, .Net Core Console apps
    * uses RabbitMQ to return responses to back end web service


## RABBIT MQ

backend and microservices require RabbitMQ to be running:
    * to start RabbitMQ: 
      * run RabbitMQ command prompt as administrator (from start menu)
      * run this command: rabbitmqctl start_app

RabbitMQ Management Console:
  * http://localhost:15672/
  * username: guest
  * password: guest

Known issue - all exchanges get deleted out of the blue
  * hoping this is due to an issue with Comodo antivirus
  * to fix: 
    * run RabbitMQ command prompt as administrator (from start menu)
    * run this command: rabbitmqctl reset
  * no idea how to prevent/detect this in prod



  