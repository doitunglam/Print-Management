<script lang="ts" setup>
import { getOrder } from '@/services/axios/api';
import { computed, onMounted, ref } from 'vue';
import Loading from "@/components/Loading.vue"
import { useRouter } from "vue-router"
import type { ProductType } from '@/types/main';
import { useGetProductById, useUpdateOrder } from '@/composables/useApi';


const router = useRouter()
const { id } = defineProps<{
    id: number
}>()

const loading = ref<boolean>(false)
const order = ref<any | null>(null)

const steps = ref<any[] | null>(null)

const getStepStatus = (step: any) => {
    if (step.status === 'Completed') return 'finish';
    if (step.status === 'Processing') return 'process';
    if (step.status === 'Aborted') return 'error';
    return 'wait';
};

const isReviewVisible = computed(() => {
    if (steps.value != null) {
        for (const step of steps.value) {
            if (step.status != 'Completed') {
                return false
            }
        }
        return true;
    } else return false;
})


const submitRating = async () => {
    await useUpdateOrder(order.value)
}

const formatStatus = (step: any) => {
    if (step.fulfilledAt) {
        return `Hoàn thành lúc ${new Date(step.fulfilledAt).toLocaleString()}`;
    }

    switch (step.status) {
        case "Aborted":
            return "Đã hủy"
        case "Processing":
            return "Đang xử lý"
        case "Others":
            console.log(step)
            return step.note;
        default:
            return "Chưa xử lý"
    }
}
const product = ref<ProductType | null>(null)


onMounted(async () => {
    try {
        loading.value = true
        const response = await getOrder(id)
        order.value = response
        console.log(order)
        const productResponse = await useGetProductById(response.productID)
        product.value = productResponse

        steps.value = response.steps.sort((stepA: any, stepB: any) => stepA.position > stepB.position)
        loading.value = false
    } catch (e) {
        loading.value = false
        console.error(e)
    }
})
</script>


<template>
    <div class="bg-white shadow-[0_4px_12px_-5px_rgba(0,0,0,0.4)] mt-4 p-4 flex-grow m-2 sm:m-8">
        <div v-if="loading">
            <Loading></Loading>
        </div>
        <div v-else-if="order == null">
            <a-result status="error" title="Có lỗi đã xảy ra" sub-title="Vui lòng thử lại sau giây lát.">
                <template #extra>
                    <a-button @click="router.push('/')" type="primary">Về trang chủ</a-button>
                    <a-button @click="router.go(0)">Thử lại</a-button>
                </template>
            </a-result>
        </div>
        <div v-else>
            <div class="flex flex-col sm:flex-row">
                <div class="sm:basis-1/3">
                    <a-steps status="process" direction="vertical">
                        <a-step v-for="step in steps" :key="step.id" :title="step.name"
                            :description="formatStatus(step)" :status="getStepStatus(step)" />
                    </a-steps>
                </div>
                <div class="sm:basis-2/3 flex flex-col sm:flex-row gap-4">
                    <img :src="product?.imageUrl"
                        class="w-full sm:w-auto sm:h-full sm:max-h-[300px] justify-center align-middle my-auto">
                    <div class="flex gap-1 flex-col justify-between sm:h-[300px] my-auto">
                        <p>
                            Tên sản phẩm: {{ product?.name }}
                        </p>
                        <p>
                            Mã thiết kế: {{ order.designID }}
                        </p>
                        <p>
                            Ngày tạo đơn hàng: {{ new Date(order.orderDate).toDateString() }}
                        </p>
                        <p>
                            Địa chỉ nhận hàng: {{ order.deliveryAddress }}
                        </p>
                        <p>
                            Tên khách hàng: {{ order.customerName }}
                        </p>
                        <p>
                            Số điện thoại nhận hàng: {{ order.phoneNumber }}
                        </p>
                        <p v-if="isReviewVisible">
                            Đánh giá: <a-rate v-model:value="order.rating" @click="submitRating" />
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped></style>