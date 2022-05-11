import { Socket } from "socket.io";
import SocketIOServer from "./SocketIOServer";
import PacketType, { PlayerDataPck, RequestMatchPck, TicketPck} from "./PacketType";
import EventEmitter from "events";
import { Ticket } from "./MatchMaker";

class PlayerEventNames {
  static AddPlayer = "addPlayer";
  static RemovePlayer = "removePlayer";
}
type playerEventHandler = (player: Player) => void;

export class PlayerManager {
  public players: Map<string, Player> = new Map<string, Player>();
  private socketToPlayerMap: Map<string, Player> = new Map<string, Player>();
  private socketioServer!: SocketIOServer;
  private playerEvent: EventEmitter = new EventEmitter();

  
  public Init(socketIOServer: SocketIOServer){
    this.socketioServer = socketIOServer;
    this.MentainPlayers();
  }

  public SendTicketToPlayer(player: Player, ticket: Ticket){
    const ticketPck: TicketPck = ticket;
    player.Socket.emit(PacketType.Ticket, ticketPck);
  }

  public OnPlayerRequestMatch(
    handler: (player: Player, requestMatchPck: RequestMatchPck) => void,
  ): void {
    this.PlayerActionWrap(PacketType.RequestMatch, handler);
  }

  public OnPlayerCancelMatch(handler: (player: Player)=> void): void{
    this.PlayerActionWrap(PacketType.CancelMatch, handler);
  }


  public OnAddPlayer(handler: playerEventHandler) {
    this.playerEvent.on(PlayerEventNames.AddPlayer, handler);
  }

  public OnRemovePlayer(handler: playerEventHandler) {
    this.playerEvent.on(PlayerEventNames.RemovePlayer, handler);
  }

  // auto mentain players
  private MentainPlayers() {
    // add player when get player data and auto remove when disconnect
    // add to players when give playerdata
    this.socketioServer.OnPacket(
      PacketType.PlayerData,
      (socket: Socket, playerData: PlayerDataPck) => {
        // add player
        const player = new Player(playerData.id, socket);
        this.socketToPlayerMap.set(socket.id, player);
        this.players.set(player.Id, player);
        // emit event
        this.playerEvent.emit(PlayerEventNames.AddPlayer, player);
      }
    );
    // remove player when disconnect
    this.socketioServer.OnDisconnect((socket: Socket, reason: string) => {
      // emit event
      const player = this.socketToPlayerMap.get(socket.id);
      if (player) {
        this.playerEvent.emit(PlayerEventNames.RemovePlayer, player);
        // delete player
        this.socketToPlayerMap.delete(socket.id);
        this.players.delete(player.Id);
      }
    });
  }

  private PlayerActionWrap(pckType: string, handler: (player: Player, args: any[])=> void){
    this.socketioServer.OnPacket(
      pckType, (socket: Socket, args)=>{
        const player = this.socketToPlayerMap.get(socket.id);
        if(player) handler(player, args);
        else throw new Error("socket id dont have coresponed player object");
      }
    )
  }
}

export class Player {
  Id: string;
  Socket: Socket;

  constructor(id: string, socket: Socket) {
    this.Id = id;
    this.Socket = socket;
  }
}
