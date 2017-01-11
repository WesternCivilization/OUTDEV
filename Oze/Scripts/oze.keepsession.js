var idTimeOut;
startKeepSessionAlive();
function startKeepSessionAlive()
{
    if (idTimeOut) clearInterval(idTimeOut);
    idTimeOut=setInterval("KeepSessionAlive()", 30000)
}

function KeepSessionAlive()
{
    var url = "/KeepSessionAlive.ashx?";
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", url, true);
    xmlHttp.send();
}