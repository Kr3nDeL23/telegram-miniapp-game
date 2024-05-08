import type BaseModel from "./baseModel";

export default interface LeagueModel extends BaseModel {
    name: string;
    availableCoin: number;
    image: string;
}
