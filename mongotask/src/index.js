    const express = require("express");

    const {
        createTask,
        findTaskById,
        editTaskById,
        deleteTaskById,
        getAllTasks,
        deleteAllTasks,
        uploadImage,
        getAllImages
        
    } = require ("./service.js");

    const {
        uploadImageModel
    } = require ("./model.js");

const { send } = require("express/lib/response.js");
    const app = express();
    const PORT = 4000;
    let task = null;
    app.use("/Images", express.static("Public/Images"))
    app.use(express.json())

    const images = [
        'localhost:4000/Images/bulma.png',
        'localhost:4000/Images/dodoria.png',
        'localhost:4000/Images/Freezer.png',
        'localhost:4000/Images/ginyu.png',
        'localhost:4000/Images/goku_normal.png',
        'localhost:4000/Images/picolo_normal.png',
        'localhost:4000/Images/vegeta_normal.png',
        'localhost:4000/Images/zarbon.png',
        'localhost:4000/Images/celula.png',
        
    ];
    uploadImagesFromLocal()

    app.get("/newTask",async function(_req, res) {
        const tasks = await getAllTasks()
        if(!task || tasks.length === 0)
        {
            return res.send("No existe un documento, por favor crea uno nuevo con el siguiente formato: \n\
                title\n description\
                ")
        }
        return res.send(await findTaskById(task.insertedId));
    })
    app.get("/allTasks",async function(_req, res) {
        const tasks = await getAllTasks();
        if(tasks.length === 0)
        {
            
            console.log("Empty: " + tasks.length)
            return res.send("No existe un documento, por favor crea uno nuevo con el siguiente formato: \n\
                title\n description\
                ")
        }
        console.log("Not empty")
        return res.send(await getAllTasks())
    })
    app.post("/newTask", async function(req, res)
    {
        try {
                const {title, description} = req.body
                if(!title || !description) throw Error("La tarea debe contener al menos un campo")
                const {creationDate, lastUpdate} = req.body
            if(creationDate || lastUpdate) throw Error("No puedes modificar los atributos de fecha y ultima actualización ")
                task = await createTask(req.body)
                res.send("Object created succesfully")
            return res.status(200).send();
        } catch (e) {
            console.log(e)
            return res.status(500).send(e.message);
        }

        
    })
    app.post("/allTasks", async function(req, res)
    {
        try {
                const {title, description} = req.body
                if(!title || !description) throw Error("La tarea debe contener al menos un campo")
                const {creationDate, lastUpdate} = req.body
            if(creationDate || lastUpdate) throw Error("No puedes modificar los atributos de fecha y ultima actualización ")
                task = await createTask(req.body)
                res.send("Object created succesfully")
            return res.status(200).send();
        } catch (e) {
            console.log(e)
            return res.status(500).send(e.message);
        }

        
    })
    app.put("/newTask", async function (req, res) {
        try {
            if(!task)
            {
                res.send("No hay tarea que editar, por favor haz una nueva")
                return Error
            }
            const {title, description} = req.body
            if(!title || !description) throw Error("No se llenaron todos los campos")
            const {creationDate, lastUpdate} = req.body
            if(creationDate || lastUpdate) throw Error("No puedes modificar los atributos de fecha y ultima actualización ")
            await editTaskById(task.insertedId, req.body)
            res.send("Tarea editada")
            
        } catch (e) {
            console.log(e)
            return res.status(500).send(e.message);
        }
    })
    app.patch("/newTask", async function (req, res) {
        try {
            if(!task)
            {
                res.send("No hay tarea que editar, por favor haz una nueva")
                return Error
            }
            const {title, description} = req.body
            if(!title && !description) throw Error("No se lleno ningun campo")
            const {creationDate, lastUpdate} = req.body
            if(creationDate || lastUpdate) throw Error("No puedes modificar los atributos de fecha y última actualización ")
            await editTaskById(task.insertedId, req.body)
            res.send("Tarea editada")
            
        } catch (e) {
            console.log(e)
            return res.status(500).send(e.message);
        }
    })
    app.delete("/newTask", async function (_req, res) {
        try {
            if(!task)
            {
                throw Error("No hay tarea que eliminar, por favor haz una nueva")
            }
            await deleteTaskById(task.insertedId)
            res.send("Tarea eliminada")
            task = null
            
        } catch (e) {
            res.status(500).send(e.message);
            console.log(e)
        }
    })
    app.delete("/allTasks", async function (_req, res) {
        try {
            const tasks = await getAllTasks();
            if(tasks === '{}')
                throw new Error("No hay tareas que eliminar");
            await deleteAllTasks();
            res.send("Tareas eliminadas")
            task = null;
        } catch (e) {
            console.log(e)
            return res.status(500).send(e.message);
        }
    })
    
    async function uploadImagesFromLocal() {
        for(const [index, imageURL] of images.entries())
        {
            await uploadImage(uploadImageModel(index, imageURL))
        }
    }
    app.get("/allIMagesAsStrings", async function (_req, res) {
        return res.send(images)
    })
    app.get("/allImages", async function (_req, res) {
        try {
            return res.send(await getAllImages())
        }catch (e){
            return res.send(e)
        }
    })
    app.post("/images", async function (req, res) {
        
    })
    app.listen(PORT, function () {
        console.log("Escuchando en puerto " + PORT)
    })

