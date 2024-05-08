export default interface NotificationModel {
    title: string;
    description: string;
    type: NotificationTypeEnum;
}
export enum NotificationTypeEnum {
    Success = 0,
    Warning = 1,
    Exception = 2,
    Information = 3,
    Waiting = 4,
};