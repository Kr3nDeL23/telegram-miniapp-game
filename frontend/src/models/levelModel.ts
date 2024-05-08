import type BaseModel from "./baseModel";

export default interface LevelModel extends BaseModel {
    level: number;
    levelType: LevelTypeEnum;

    value: number;
    availableCoin: number;
}
export enum LevelTypeEnum {
    LimitEnergyLevel = 0,
    MultipleClickLevel = 1,
    RechargeEnergyLevel = 2,
    RoBotLevel = 3,
};