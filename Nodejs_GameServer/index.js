const { WebSocketServer } = require("ws");

const sockserver = new WebSocketServer({ port: 3030 });
console.log('server run on *:3030')

sockserver.on("connection", (ws) => {
 
  ws.id = Date.now();
  console.log("New client connected!");
  broadcast(ws, 'Idle|(0,0,0)');

  for(let client of sockserver.clients){
    if(client.id == ws.id) continue;
    
    ws.send(`${client.id}|${client.state}`);
  }
  
  ws.on("close", ()=>{
    broadcast(ws, 'CLOSE|(0,0,0)')
  });
  
  ws.on("message", (data) => {
    let dataStr = data.toString();
    broadcast(ws, dataStr);
  });
  ws.onerror = function () {
    console.log("websocket error");
  };
});

function broadcast(ws, data){
    console.log(`${ws.id}|${data}`);
    ws.state = data;
    for(let client of sockserver.clients){
        if(client.id == ws.id) continue;
        client.send(`${ws.id}|${data}`);
    }
}
