<template>
    <div class="mx-auto page_information flex-center flex-column text-center">
        <h1 class="text-color fs-large fw-bold">Swap & Transfer</h1>
        <p class="description-color fs-small fw-bold mt-3">
            In this section you can convert your coins to other currencies
        </p>
    </div>
    <section class="section_transfer mt-5">
        <div class="box-shadow bg-card p-3 rounded-1">
            <div class="d-flex flex-column">
                <span class="fs-small text-color fw-bold">Transfer</span>
                <span class="fs-secondary-small description-color fw-normal mt-1">Your Balance Is <b>{{
                    $filters.numberFormat(user?.balanceCoin ?? 0) }}</b></span>
            </div>


            <div class="d-flex flex-column mt-1">
                <div class="d-flex flex-column mt-2 profile_transfer_input bg-card box-shadow rounded-1 p-2">
                    <label for="count_coin" class="fs-secondary-small description-color fw-bold">Count Coin</label>
                    <input v-model.number="transferCount" @keypress="checkIsNumber($event)" id="count_coin"
                        placeholder="10,000 Minimal" type="number" class="fs-small text-color fw-bold mt-1">
                </div>
                <div class="d-flex flex-column mt-2 profile_transfer_input bg-card box-shadow rounded-1 p-2">
                    <label for="user_id" class="fs-secondary-small description-color fw-bold">user id</label>
                    <input v-model.number="transferUserId" @keypress="checkIsNumber($event)" id="user_id"
                        placeholder="19817281" type="number" class="fs-small text-color fw-bold mt-1">
                </div>
                <button @click="transferSubmit" class="bg-theme text-color mt-3 fs-medium p-2 fw-bold rounded-1">
                    Transfer
                </button>
            </div>
        </div>
    </section>
    <div class="section_swap mt-3">
        <div class="bg-card p-3 rounded-1 box-shadow">
            <div class="d-flex flex-column">
                <span class="fs-small text-color fw-bold">Swap Coin</span>
                <span class="fs-secondary-small description-color fw-normal mt-1">Your Balance Is <b>{{
                    $filters.numberFormat(user?.balanceCoin ?? 0) }}</b></span>
            </div>
            <div class="d-flex flex-column mt-3">
                <div class="d-flex flex-column profile_transfer_input bg-card box-shadow rounded-1 p-2">
                    <span class="fs-secondary-small description-color fw-bold">Select Currency</span>

                    <ul class="mt-1">

                        <li v-if="showCurrencyList">
                            <ul class="currency_list">
                                <li v-for="(currency, index) in currencies" :key="index"
                                    class="d-flex align-items-center py-2"
                                    @click="currentCurrency = currency; showCurrencyList = false">
                                    <img width="25px" :src="$filters.serverLinkFormat(currency.image)" alt="">
                                    <span class="ml-1 fs-small fw-bold text-color">{{ currency.name }}</span>
                                </li>
                            </ul>
                        </li>
                        <li v-else-if="currentCurrency" @click="showCurrencyList = true"
                            class="d-flex align-items-center py-2">
                            <img width="25px" :src="$filters.serverLinkFormat(currentCurrency.image)" alt="">
                            <span class="ml-1 fs-small fw-bold text-color">{{ currentCurrency.name }}</span>
                        </li>

                    </ul>
                </div>
                <div class="d-flex flex-column mt-2 profile_transfer_input bg-card box-shadow rounded-1 p-2">
                    <label for="wallet_address" class="fs-secondary-small description-color fw-bold">wallet
                        address</label>
                    <input v-model="swapWallet" id="wallet_address" placeholder="Trx Wallet Address" type="text"
                        class="fs-small text-color fw-bold mt-1">
                </div>
                <div class="d-flex flex-column mt-2 profile_transfer_input bg-card box-shadow rounded-1 p-2">
                    <label for="swap_coin" class="fs-secondary-small description-color fw-bold">Count Coin</label>
                    <input v-model.number="swapCount" @keypress="checkIsNumber($event)" id="swap_coin"
                        placeholder="Count Coin For Swap" type="number" class="fs-small text-color fw-bold mt-1">
                </div>
                <span v-if="currentCurrency" class="description-color fs-small fw-bold mt-3">
                    To Doller = ${{ $filters.numberFormat(getSwapAmount()) }}
                </span>
                <button @click="swapRequest" class="bg-theme text-color mt-3 fs-medium p-2 fw-bold rounded-1">
                    swap
                </button>
                <div class="line mt-4"></div>
                <div class="description mt-2">
                    <p class="description-color fs-secondary-small fw-normal">
                        The operation of converting coins to other currencies is irreversible. Please fill in the values
                        ​​carefully. Before doing this, read our terms and conditions because we have no responsibility
                        for not reading them.
                    </p>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, inject } from 'vue'

import UserService from '@/services/userService';
import type UserModel from '@/models/userModel';

import { NotificationTypeEnum } from '@/models/notificationModel';
import type CurrencyModel from '@/models/currencyModel';
import CurrencyService from '@/services/currencyService';

const { createNotification } = inject("notification");

const user = ref<UserModel>();
const currentCurrency = ref<CurrencyModel | null>(null);

const showCurrencyList = ref<boolean>(false);
const swapCount = ref<number>();
const swapWallet = ref<string>("");
const transferCount = ref<number>();
const transferUserId = ref<number>();
const currencies = ref<Array<CurrencyModel>>(new Array<CurrencyModel>());

const userService = new UserService();
const currencyService = new CurrencyService();


const fetchRequest = () => {
    const userService = new UserService();

    userService.profile()
        .then(result => user.value = result)
    currencyService.getList()
        .then(result => {
            currencies.value = result;
            if (result.length > 0)
                currentCurrency.value = result[0];
        })
};
const swapRequest = () => {
    if (!user.value || !swapCount.value) {

        createNotification({
            title: "Input Error",
            description: "Please fill all the fields correctly",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }

    if (user.value.balanceCoin < swapCount.value) {
        createNotification({
            title: "Balance Error",
            description: "Balance is not enough",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    if (swapCount.value < 10000000) {
        createNotification({
            title: "Count Error",
            description: "The minimum amount is 10,000,000 coins",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    if (!swapWallet.value.toLowerCase().startsWith("t")) {
        createNotification({
            title: "Wallet Error",
            description: "Wallet Address is not valid (Trc20)",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    createNotification({
        title: "Comming Soon",
        description: "This section will be completed soon",
        type: NotificationTypeEnum.Information,
    });
};
const checkIsNumber = (event: KeyboardEvent) => {
    const keysAllowed: string[] = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.'];
    const keyPressed: string = event.key;

    if (!keysAllowed.includes(keyPressed)) {
        event.preventDefault()
    }
    getSwapAmount();
};
const getSwapAmount = () => {
    if (!currentCurrency.value || !swapCount.value) return "0";

    return (swapCount.value / 200000) * currentCurrency.value.price;


}
const transferSubmit = () => {
    if (!user.value) return;

    if (!transferCount.value || transferCount.value == 0 || !transferUserId.value || transferUserId.value == 0) {
        createNotification({
            title: "Input Error",
            description: "Please fill all the fields correctly",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    if (user.value.balanceCoin < transferCount.value) {
        createNotification({
            title: "Balance Error",
            description: "Balance is not enough",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    if (transferCount.value < 10000) {
        createNotification({
            title: "Count Error",
            description: "The minimum amount is 1000 coins",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    createNotification({
        title: "please wait",
        description: "The request is in progress ...",
        type: NotificationTypeEnum.Waiting,
    });
    userService.transfer(transferCount.value, transferUserId.value)
        .then(result => {
            user.value = result;
            createNotification({
                title: "Transfer Successlly",
                description: "Coins have been successfully sent to the user",
                type: NotificationTypeEnum.Success,
            });
        })
        .catch(except => {
            if (except.responseStatus == 400) {
                createNotification({
                    title: "Balance Error",
                    description: "Balance is not enough",
                    type: NotificationTypeEnum.Exception,
                });
                return;
            }
            else if (except.responseStatus == 404) {
                createNotification({
                    title: "User Id Invalid",
                    description: "Can not found user with this id",
                    type: NotificationTypeEnum.Exception,
                });
                return;
            }
            createNotification({
                title: "Transfer Error",
                description: "Can not Transfer now",
                type: NotificationTypeEnum.Exception,
            });
        })
};
fetchRequest();
</script>

<style scoped>
.line {
    width: 100%;
    height: 1px;
    background: rgba(var(--border-color), 0.1);
}
</style>