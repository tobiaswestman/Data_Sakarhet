// Once the HTML document has been completely loaded, execute the function
document.addEventListener('DOMContentLoaded', function() {

    // Add event listeners to specific buttons for authentication and registration actions
    document.getElementById('login-btn').addEventListener('click', showLoginForm);
    document.getElementById('register-btn').addEventListener('click', showRegisterForm);
    document.getElementById('login-submit-btn').addEventListener('click', login);
    document.getElementById('register-submit-btn').addEventListener('click', register);
    document.getElementById('logout-btn').addEventListener('click', logout);
    document.getElementById('cancel-btn1').addEventListener('click', showAuthButtons);
    document.getElementById('cancel-btn2').addEventListener('click', showAuthButtons);

    // Establish a SignalR connection to the server hub to receive temperature data
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7087/temperatureDataHub")
        .build();

    let temperatureList = [];

    // Listen for temperature updates from the server and handle the received data
    connection.on("ReceiveTemperature", (temperature) => {
        let tempValue = typeof temperature === 'string' ? parseFloat(temperature) : temperature;

        temperatureList.unshift(tempValue);

        if (temperatureList.length > 20) {
            temperatureList.pop();
        }

        updateFrontEnd(temperatureList);
    });

    // Start the SignalR connection
    connection.start()
        .catch(err => console.error(err.toString()));


    // Once the window is loaded, set up the initial view based on user authentication status and role
    window.onload = function () {
        
        const isTokenValid = verifyToken();
        console.log(isTokenValid);
        const token = localStorage.getItem('token');

        if (!token) {
            showAuthButtons();
        } else {
            showDataView();
            setRoleStyling();
        }
    }

    // Calculate the average temperature from a list of generated temperatures
    function getAvgTemp(temperatureList){
        if(temperatureList.length === 0) return 0;

        let sum = temperatureList.reduce((accumulator, currentValue) => accumulator + currentValue, 0);
        return sum / temperatureList.length;
    }

    
    // Update the frontend with the latest list of temperatures and calculated average
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
    }

     // functions to show/hide various sections/forms of the application
    function showLoginForm() {
        hideAllSections();
        document.getElementById("login-form").style.display = "block";
    }

    function showRegisterForm() {
        hideAllSections();
        document.getElementById("register-form").style.display = "block";
    }

    function showDataView() {
        hideAllSections();
        document.getElementById("data-view").style.display = "block";
    }

    function showAuthButtons() {
        hideAllSections();
        document.getElementById("auth-buttons").style.display = "block";
    }

    function hideAllSections() {
        document.getElementById("auth-buttons").style.display = "none";
        document.getElementById("login-form").style.display = "none";
        document.getElementById("register-form").style.display = "none";
        document.getElementById("data-view").style.display = "none";
    }

    // Function to handle user login, makes fetch call to server endpoint, recieves JWT-token, saves token in localstorage
    async function login() {
        var email = document.getElementById("login-email").value;
        var password = document.getElementById("login-password").value;
    
        try {
            const response = await fetch('https://localhost:7087/api/account/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ 
                    email: email, 
                    password: password 
                })
            });
    
            if (!response.ok) {
                throw new Error('Login failed');
            }
    
            const data = await response.json();
    
            localStorage.setItem('token', data.token);
    
            document.getElementById("login-form").style.display = "none";
            showDataView();
    
        } catch (error) {
            alert(error.message);
        }
    
        setRoleStyling();
    }
    
    // Set the styling of the application based on the role of the logged-in user, admins have a red background, users have a blue background
    async function setRoleStyling() {
        const role = await getUserRole();
    
        document.body.className = "";  
        switch (role) {
            case 'admin':
                document.body.classList.add('admin-role');
                break;
            case 'user':
                document.body.classList.add('user-role');
                break;
            default:
                document.body.style.backgroundColor = "white";
                break;
        }
    }

    // Function to handle user registration
    async function register() {
        var email = document.getElementById("register-email").value;
        var password = document.getElementById("register-password").value;

        try {
            const response = await fetch('https://localhost:7087/api/account/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ 
                    email: email, 
                    password: password 
                })
            });

            if (!response.ok) {
                const responseData = await response.json();
                throw new Error(responseData.message || 'Registration failed');
            }

            alert('Registration successful! Please log in.');
            document.getElementById("register-form").style.display = "none";
            document.getElementById("login-form").style.display = "block";

        } catch (error) {
            alert(error.message);
        }
    }

    // Function to handle user logout, requires auth, removes jwt token from localstorage
    async function logout() {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch('https://localhost:7087/api/account/LogOut', {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + token,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Logout failed');
            }

            localStorage.removeItem('token');
            document.body.className = '';
            showAuthButtons();

        } catch (error) {
            alert(error.message);
        }
    }

    // Function to verify the validity of the stored authentication token through fetch call to our backend api
    async function verifyToken() {
        const token = localStorage.getItem('token');

        if (!token) {
            console.log('No token found');
            return false;
        }

        try {
            const response = await fetch('https://localhost:7087/api/account/VerifyToken', {
                headers: {
                    'Authorization': 'Bearer ' + token
                }
            });

            if (!response.ok) {
                throw new Error('Token verification failed');
            }

            const data = await response.json();
            return data.Status === 'Valid';

        } catch (error) {
            console.error(error);
            localStorage.removeItem('token');
            return false;
        }
    }

    // Function to fetch the role of the currently logged-in user
    async function getUserRole() {
        const token = localStorage.getItem('token');
        try {
            const response = await fetch("https://localhost:7087/api/account/userRole", {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + token,
                    'Content-Type': 'application/json'
                }
            });
    
            if (!response.ok) {
                throw new Error('Failed to fetch user role');
            }
    
            const role = await response.json();
            return role.role;

        } catch (error) {
            console.error('Error fetching user role:', error);
            return null;
        }
    }
    
});

