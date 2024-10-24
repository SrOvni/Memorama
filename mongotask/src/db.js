const { MongoClient} = require ("mongodb");
const DB_URL ="mongodb://localhost:27017/"
const client = new MongoClient(DB_URL);
const database = client.db ("Task");
const Task = database.collection("task");
module.exports = {
    Task,
};