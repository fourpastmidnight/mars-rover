const baseUrl = process.env.REACT_APP_MARSROVERPHOTOS_WEBAPI_URL;

export function getRandomRoverPhotos(numPhotos) {
    // TODO: Find a way to determine the number of available photos -- for now hard-coding value to 552.
    const skip = Math.floor(Math.random() * 548) + 1;
    return fetch(`${baseUrl}?skip=${skip}&take=${numPhotos}`)
        .then(handleResponse)
        .catch(handleError);
}

export function getRoverPhotosByRoverOnDate(rover, dateTaken) {
    return fetch(`${baseUrl}/${rover}/${dateTaken.toString('yyyy-MM-dd')}`)
        .then(handleResponse)
        .catch(handleError);
}

export function getAllRoverPhotosOnDate(dateTaken) {
    return fetch(`${baseUrl}/${dateTaken.toString('yyyy-MM-dd')}`)
    .then(handleResponse)
    .catch(handleError);
}

export function getAllRoverPhotos(rover) {
    return fetch(`${baseUrl}/${rover}`)
        .then(handleResponse)
        .catch(handleError);
}

export async function handleResponse(response) {
    if (response.status === 200) return await response.json();
    if (response.status === 204) return ({ "photos" : [] });

    throw new Error("Network response was not ok!\n" + await response.text()) // TODO: Handle potential additional errors
}

export function handleError(error) {
    // TODO: Actually handle the error -- for now, just log to the console.
    // eslint-dissable-next-line no-console
    console.error("API call failed. " + error);
    throw error;
}