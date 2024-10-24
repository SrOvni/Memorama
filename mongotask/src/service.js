const { 
    createTaskModel,
    uploadImageModel
} = require("./model.js");
const { MongoClient } = require("mongodb");

let Task;

async function init() {
    const DB_URL = "mongodb://localhost:27017";
    const client = new MongoClient(DB_URL);

    try {
        // Conectarse a la base de datos
        await client.connect();
        console.log("Conectado a MongoDB");

        // Seleccionar la base de datos y la colección
        const database = client.db("Task");
        Task = database.collection("task");

    } catch (e) {
        console.error("Error conectando a MongoDB:", e);
    }
}

// Crear una nueva tarea en la colección
async function createTask(task) { //post
    if (!Task) {
        await init();  // Asegurarse de que la conexión esté lista
    }
    const model = createTaskModel(task.title, task.description, Date(), Date());
    const result = await Task.insertOne(model);
    return result;
}

async function findTaskById(id) { //get
    if (!Task) {
        await init();
    }
    const result = await Task.findOne({ _id: id }); //GET
    return result;
}

async function editTaskById(id, dataToUpdate) { //Patch
    if (!Task) {
        await init();
    }
    const result = await Task.updateOne(
        { _id: id }, 
        {
            $set: {
                "title": dataToUpdate.title, 
                "description": dataToUpdate.description, 
                "lastUpdate": Date()
            }
        }
    )
    return result;
}

async function deleteTaskById(id) { //Delete
    if (!Task) {
        await init();
    }
    const result = await Task.deleteOne({ _id: id });
    return result;
}
async function getAllTasks() {
    if(!Task)
    {
        await init()
    }
    const tasks = await Task.find({}).toArray();
    
    return tasks
}
async function deleteAllTasks() {
    if(!Task)
    {
        await init()
    }
    const result = await Task.deleteMany({});
    return result;
}
//----------------------------------------------------------------------
let Images = null;
async function initMemoryDB() {
    const DB_URL = "mongodb://localhost:27017";
    const client = new MongoClient(DB_URL);

    try {
        // Conectarse a la base de datos
        await client.connect();
        
        // Seleccionar la base de datos y la colección
        const database = client.db("MemoryImages");
        Images = database.collection("Images");
        console.log("Using MemoryImages database");

    } catch (e) {
        console.error("Error conectando a MongoDB:", e);
    }
}
async function uploadImage(character, imageURL) {
    if(!Images)
    {
        await initMemoryDB()
    }
    const model = uploadImageModel(character, imageURL)
    return await Images.insertOne(model)
}
async function getAllImages() {
    if(!Images)
    {
        await initMemoryDB()
    }
    const images = await Images.find({}).toArray();
    return images
}
module.exports = {
    createTask,
    findTaskById,
    editTaskById,
    deleteTaskById,
    getAllTasks,
    deleteAllTasks,
    uploadImage,
    getAllImages
};
