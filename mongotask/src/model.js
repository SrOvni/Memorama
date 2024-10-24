function createTaskModel (title, description, creationDate, lastUpdate) {
    return {
        title: title,
        description : description,
        creationDate : creationDate,
        lastUpdate : lastUpdate
    };
}
function uploadImageModel(character, imageURL)
{
    return {
        charatcer: character,
        imageURL: imageURL
    }
}

module.exports = {
    createTaskModel,
    uploadImageModel
};