<template>
    <div class="flex-center flex-column">
        <div v-if="users && users.length == 0" class="flex-center py-3">
            <span class="text-color fs-small fw-bold">Can not found users</span>
        </div>

        <UserCard v-if="users" v-for="(   user, index   ) in    users   " :key="index" :user="user" />
        <UserCard v-else v-for="(   user, i   ) in    [null, null, null]   " :key="i" :user="user" />
    </div>
</template>

<script setup lang="ts">
import type UserModel from '@/models/userModel';

import UserCard from '@/components/cards/UserCard.vue';
import SkeletonLoader from '@/components/utilities/SkeletonLoader.vue';

interface Props {
    users: Array<UserModel> | null
}
const props = defineProps<Props>()

</script>