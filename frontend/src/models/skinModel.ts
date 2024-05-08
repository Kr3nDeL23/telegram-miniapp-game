import type BaseModel from "./baseModel";

export default interface SkinModel extends BaseModel {
    title: string;
    availableCoin: number;
    image: string;
    isBought: boolean;
}