# FunctionalCrudForms

This  is a demo app of CRUD forms in F#, a functional langauge. The user can create, read, update, delete, and search for employees.

## Projects and Directories

This app consists of several projects:
  * __/node__ folder
    * used to load UI, but not data
    * NextJS application
      * uses React with either client-side or server-side rendering
    * references JS files produced by Fable project
    * to run: 
      * cd node
      * npm run dev
  * __/fable__ folder
    * Fable is a tool that transpiles F# into JS
    * these JS files can be  referenced by the front end
    * to build:
      * npm run-script build
      * transpiles to a JS bundle file in the node project
    * to start:
      * npm run-script start
      * serves content from localhost:8080
    * to debug:
      * npm run-script start
      * this doesn't work as a pre-build step because I can't get it to run in the background
      * then press F5 in VS Code
      * Note:
        * this will only hit breakpoints in your Fable F# code
        * if you want to hit breakpoints in the node project, you will have to debug that project
        * example: debugging an issue w/ client-side rendering
        * you can only debug one project at a time, since only 1 process can attach to the browser.
    * __/client__ folder
      * contains code that is only run on the client-side
    * __/shared__ folder
      * contains code that is run on both client and server
      * example: validation logic
      * client runs the transpiled JS form of the F# code in the browser
      * microservices reference the compiled F# DLL's
  * __/backend-crud__ folder: 
    * back end web service
    * used to load or save data, but not for loading the UI
    * F#, .Net Core
    * uses RabbitMQ to call microservices
      * only calls microservices, doesn't do any logic itself
      * returns response from microservice back to client
  * __/microservices__ folder:
    * microservices that run on the back end
    * F#, .Net Core Console apps
    * uses RabbitMQ to return responses to back end web service
    * all logic, loading, and saving, are done by microservices, not back end


## Rabbit MQ

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



  
