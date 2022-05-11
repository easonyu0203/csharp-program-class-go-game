import { io } from "socket.io-client";
import dotenv from "dotenv";
dotenv.config();
import PacketType, {
  GameDataNames,
  PlayerDataPck,
  RequestMatchPck,
  TicketPck,
} from "./PacketType";

if (process.env.HOST == undefined || process.env.PORT == undefined) {
  throw new Error(".env not set HOST, PORT");
}
let url = `http://${process.env.HOST}:${process.env.PORT}`;

const socket = io(url);
const playerId = process.argv[process.argv.length - 1];
test();

function testConnection() {
  socket.on("connect", () => console.log("Connect to server"));
}

function testPlayerData() {
  console.log(`send player data`);
  const playerData: PlayerDataPck = { id: playerId };
  socket.emit(PacketType.PlayerData, playerData);
}

function testMatch() {
  console.log("request match");
  const requestMatchPck: RequestMatchPck = {};
  socket.emit(PacketType.RequestMatch, requestMatchPck);
  socket.on(PacketType.Ticket, (tickerPck: TicketPck) => {
    console.log(
      `get ticket: ${tickerPck.MethodSpecificData.packetType}, ${tickerPck.p2pConnectMethod}`
    );

    testRelay();
  });
}

function testRelay() {
  socket.emit(PacketType.RelayData, "hey how you doing");
  socket.on(PacketType.RelayData, (data) => {
    console.log(`get game data: ${data}`);
    const dd = data;
  });
}

function test() {
  testConnection();
  testPlayerData();
  testMatch();
}
