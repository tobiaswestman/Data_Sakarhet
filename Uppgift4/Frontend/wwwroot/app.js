const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7087/temperatureDataHub")
    .build();

let temperatureList = [];

connection.on("ReceiveTemperature", (temperature) => {
    // Konvertera temperature till en siffra om det är en sträng
    let tempValue = typeof temperature === 'string' ? parseFloat(temperature) : temperature;

    temperatureList.unshift(tempValue);
    console.log("Received temperature:", tempValue);

    if (temperatureList.length > 20) {
        temperatureList.pop();
    }

    updateFrontEnd(temperatureList);
});

function getAvgTemp(temperatureList){
    if(temperatureList.length === 0) return 0;

    let sum = temperatureList.reduce((accumulator, currentValue) => accumulator + currentValue, 0);
    console.log("Total sum:", sum);
    return sum / temperatureList.length;
}

connection.start()
    .catch(err => console.error(err.toString()));

function updateFrontEnd(temps) {
    const listElement = document.getElementById("temperatureList");
    listElement.innerHTML = '';

    temps.forEach(temp => {
        const listItem = document.createElement('li');
        listItem.textContent = temp;
        listElement.appendChild(listItem);
    });

    const avgTempElement = document.getElementById("averageTemperature");
    const avgValue = getAvgTemp(temps);
    avgTempElement.textContent = avgValue.toFixed(2);
    console.log("Average temperature updated:", avgValue.toFixed(2));
}

