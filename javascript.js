// I got help from chatgpt for the javascript configurations here.
function updateClock() {
    const now = new Date();
    const hours = now.getHours().toString().padStart(2, '0');
    const minutes = now.getMinutes().toString().padStart(2, '0');
    const seconds = now.getSeconds().toString().padStart(2, '0');
    const timeString = `${hours}:${minutes}:${seconds}`;
    document.getElementById("liveClock").textContent = timeString;
}

setInterval(updateClock, 1000);
window.onload = function() {
    updateClock();
};


document.getElementById("loginButton").addEventListener("click", function() {
    let username = document.getElementById("username").value;
    let password = document.getElementById("password").value;

    if (username === "admin" && password === "admin") {
        window.location.href = "table.html"; 
    } else {
        alert("Hatali giris!");
        document.getElementById("username").style.border = "2px solid red";
        document.getElementById("password").style.border = "2px solid red";
    }
});


document.addEventListener("keydown", function(event) {
    if (event.key === "Enter") {
        document.getElementById("loginButton").click();
    }
});


