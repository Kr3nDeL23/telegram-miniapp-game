import type BaseModel from "./baseModel";
import type LeagueModel from "./leagueModel";

export default interface SquadModel extends BaseModel {
    name: string;
    userName: string;
    balanceCoin: number;
    image: string;
    leagueId: string;
    league: LeagueModel;
}