class PacketType {
    static Disconnect = "disconnect";
    static Connection = "connection";
    static PlayerData = "PlayerData";
    static RequestMatch = "requestMatch";
    static CancelMatch = "cancelMatch";
    static RelayData = "relayData";
    static Ticket = "ticket";
}

export interface PlayerDataPck {
    id: string;
}

export interface RequestMatchPck {

}

export interface RespondMatchPck {
    status: string;
}

export interface TicketPck{
    p2pConnectMethod: string;
    MethodSpecificData: any;
}

export interface GameDataPck{
    DatatypeName: string;
    Message: string | void;
    Winner: number | void;
    XIndex: number | void;
    YIndex: number | void;
}

export class GameDataNames{
    static HandShake: string = "handShake";
    static PlaceStone: string = "placeStone";
    static GameOver: string = "gameOver";
}

export default PacketType