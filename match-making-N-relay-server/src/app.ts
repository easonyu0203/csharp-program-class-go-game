import { Socket } from "socket.io";
import SocketIOServer from "./SocketIOServer";
import PacketType, { PlayerDataPck, RequestMatchPck } from "./PacketType";
import { Player, PlayerManager } from "./PlayerManager";
import MatchMaker from "./MatchMaker";
import Relayer from "./Relayer";
import EventEmitter from "events";
EventEmitter.prototype.setMaxListeners(20);

const socketIOServer = new SocketIOServer();
const playerManager = new PlayerManager();
const matchMaker = new MatchMaker();
const relayer = new Relayer();
socketIOServer.Init();
playerManager.Init(socketIOServer);
matchMaker.Init(playerManager, relayer);
relayer.Init(socketIOServer);

socketIOServer.StartServer();

test();

function test() {
  function testConnection() {
    socketIOServer.OnConnect((socket) =>
      console.log(`new connection, connect cnt: ${socketIOServer.sockets.size}`)
    );
    socketIOServer.OnDisconnect((socket: Socket, reason: string) =>
      console.log(`socket disconnect, socket id ${socket.id} reason: ${reason}`)
    );
  }

  function testPlayerData() {
    socketIOServer.OnPacket(
      PacketType.PlayerData,
      (socket: Socket, playerData: PlayerDataPck) => {
        console.log(
          `Get player data:\nplayer id: ${playerData.id}\nsocket id:${socket.id}`
        );
      }
    );
  }

  function testPlayerAddRemove() {
    playerManager.OnAddPlayer((player: Player) =>
      console.log(`player added player id: ${player.Id}`)
    );
    playerManager.OnRemovePlayer((player: Player) =>
      console.log(`player removed player id ${player.Id}`)
    );
  }

  function TestMatchMaking() {
    playerManager.OnPlayerRequestMatch(
      (player: Player, requestMatchPck: RequestMatchPck) => {
        console.log(`player ${player.Id} request match`);
      }
    );
    playerManager.OnPlayerCancelMatch((player: Player) => {
      console.log("player cancel match");
    });
  }

  testConnection();
  testPlayerData();
  testPlayerAddRemove();
  TestMatchMaking();
}
