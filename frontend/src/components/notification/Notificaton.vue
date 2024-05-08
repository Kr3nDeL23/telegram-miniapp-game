<template>
    <div class="notification w-full p-2">
        <div :class="notification ? `notification_${NotificationTypeEnum[notification.type].toLowerCase()}` : ''"
            class="notification_card w-full rounded-1 box-shadow p-2">
            <div class="d-flex align-items-center">
                <div class="flex-center mr-2" v-if="notification.type == NotificationTypeEnum.Success">
                    <img width="50px" src="@/assets/images/icons/success.png" alt="">
                </div>
                <div class="flex-center mr-2" v-else-if="notification.type == NotificationTypeEnum.Waiting">
                    <img width="50px" src="@/assets/images/icons/hourglass.png" alt="">
                </div>
                <div class="flex-center mr-2" v-else-if="notification.type == NotificationTypeEnum.Warning">
                    <img width="50px" src="@/assets/images/icons/warning.png" alt="">
                </div>
                <div class="flex-center mr-2" v-else-if="notification.type == NotificationTypeEnum.Information">
                    <img width="50px" src="@/assets/images/icons/information.png" alt="">
                </div>
                <div class="flex-center mr-2" v-else-if="notification.type == NotificationTypeEnum.Exception">
                    <img width="50px" src="@/assets/images/icons/exception.png" alt="">
                </div>
                <div class="d-flex flex-column">
                    <span class="text-color fs-small fw-bold">{{ notification.title }}</span>
                    <span class="text-color fs-small fw-normal mt-2">{{ notification.description }}</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
import type NotificationModel from '@/models/notificationModel';
import { NotificationTypeEnum } from '@/models/notificationModel';

interface Props {
    notification: NotificationModel,
}

const props = defineProps<Props>();
</script>

<style scoped>
@keyframes ShowNotification {
    0% {
        bottom: -200px;
    }

    100% {
        bottom: 0;
    }
}

.notification {
    left: 0;
    bottom: 0;
    position: fixed;
    animation: ShowNotification alternate var(--transition-fast);
}

.notification_card {
    border: 2px solid rgba(var(--border-color), 0.5);
}

.notification_success {
    background: rgba(var(--color-success));
}

.notification_exception {
    background: rgba(var(--color-error));
}

.notification_information {
    background: rgba(var(--color-info));
}

.notification_warning {
    background: rgba(var(--color-warning));
}
</style>