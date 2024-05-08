import type BaseModel from "./baseModel";
import type LeagueModel from "./leagueModel";
import type LevelModel from "./levelModel";
import type SkinModel from "./skinModel";
import type SquadModel from "./squadModel";

export default interface UserModel extends BaseModel {
    name: string;
    telegramId: number;
    balanceCoin: number;
    availableEnergy: number;
    lastClickDate: Date;
    limitEnergyLevelId: string;
    limitEnergyLevel: LevelModel;
    multipleClickLevelId: string;
    multipleClickLevel: LevelModel;
    rechargeEnergyLevelId: string;
    roBotLevel: LevelModel;
    roBotLevelId: string;
    rechargeEnergyLevel: LevelModel;
    image: string;
    squadId: string;
    squad: SquadModel;
    leagueId: string;
    league: LeagueModel;
    parentId: string;
    skinId: string;
    skin: SkinModel;
}