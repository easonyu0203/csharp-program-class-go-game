import express from "express";
import { createServer } from "http";
import { Server, Socket } from "socket.io";
import dotenv from "dotenv";
dotenv.config();
import PacketType from "./PacketType";

class SocketIOServer {
  public sockets: Map<string, Socket> = new Map<string, Socket>();

  private io: Server;
  private httpServer;

  constructor() {
    // create server
    const app = express();
    const httpServer = createServer(app);
    const io = new Server(httpServer, {
      /* options */
    });
    this.io = io;
    this.httpServer = httpServer;
    // mentain list of connect  socket
    this.MentainConnectedSockets();
  }

  public Init(){}


  public StartServer() {
    // check host & port is provide
    if (process.env.HOST == undefined || process.env.PORT == undefined) {
      throw new Error(".env not set HOST, PORT");
    }
    // listening for connection
    this.httpServer.listen(process.env.PORT);
    console.log(`listening at ${process.env.HOST}:${process.env.PORT}...`);
  }

  public OnConnect(handler: (socket: Socket) => void) {
    this.io.on(PacketType.Connection, handler);
  }

  public OnDisconnect(handler: (socket: Socket, reason: string) => void) {
    this.io.on(PacketType.Connection, (_socket: Socket) => {
      _socket.on(PacketType.Disconnect, (reason: string) => {
        handler(_socket, reason);
      });
    });
  }

  public OnPacket(
    packetType: string,
    handler: (socket: Socket, ...args: any[]) => void
  ) {
    this.io.on(PacketType.Connection, (socket: Socket) => {
      socket.on(packetType, (args: any[]) => {
        handler(socket, args);
      });
    });
  }

  private MentainConnectedSockets() {
    // add socket to list when connect
    this.OnConnect((socket: Socket) => this.sockets.set(socket.id, socket));
    // remove socket on list when disconnect
    this.OnDisconnect((socket: Socket, reason: string) => {
      this.sockets.delete(socket.id);
    });
  }
}

export default SocketIOServer;
