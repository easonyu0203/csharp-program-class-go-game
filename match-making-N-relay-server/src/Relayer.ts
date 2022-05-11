import { Socket } from "socket.io";
import PacketType from "./PacketType";
import SocketIOServer from "./SocketIOServer";


interface ChannelData{
    name: string;
    s1ID: string;
    s2ID: string;
}

export default class Relayer{
    private socketIOServer!: SocketIOServer;
    private channels: Map<string, ChannelData> = new Map<string, ChannelData>();
    private socketToChannel: Map<string, string> = new Map<string, string>();

    public Init(socketIOServer: SocketIOServer){
        this.socketIOServer = socketIOServer;
        // Relaying
        this.Relaying();
    }

    private Relaying() {
        this.socketIOServer.OnPacket(PacketType.RelayData, (socket: Socket, args)=>{
            if(this.socketToChannel.has(socket.id)){
                const channel = this.channels.get(this.socketToChannel.get(socket.id)!);
                socket.to(channel!.name).emit(PacketType.RelayData, args);
            }
        });
    }

    public P2PRelay(s1: Socket, s2: Socket){
        // clean up if already have those sockets
        this.cleanUp(s1);
        this.cleanUp(s2);
        // create Relay channel
        const name = `RelayChannel${this.channels.size+1}`; 
        const channel: ChannelData = {name: name, s1ID: s1.id, s2ID: s2.id};
        // add to channel
        this.channels.set(name, channel);
        this.socketToChannel.set(s1.id, name);
        this.socketToChannel.set(s2.id, name);
        s1.join(name);
        s2.join(name);
    }

    private cleanUp(s: Socket) {
        const channelName = this.socketToChannel.get(s.id);
        if(channelName){
            const channel = this.channels.get(channelName);
            if(channel != undefined){
                const s1 = this.socketIOServer.sockets.get(channel.s1ID);
                const s2 = this.socketIOServer.sockets.get(channel.s1ID);
                // clean up
                this.channels.delete(channelName);
                if(s1) this.socketToChannel.delete(s1.id);
                if(s2) this.socketToChannel.delete(s2.id);
            }

        }
    }
}