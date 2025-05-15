<script setup lang="ts">
import PageLayout from "@/layouts/PageLayout.vue"
import HeroSection from "@/components/home/HeroSection.vue"
import TopProducts from "@/components/home/TopProducts.vue"
import TopDesign from "@/components/home/TopDesign.vue"
import StatisticalNumber from "@/components/home/StatisticalNumber.vue"
import Testimonial from "@/components/home/Testimonial.vue"
</script>

<template>
  <PageLayout>
    <HeroSection></HeroSection>
    <TopProducts></TopProducts>
    <TopDesign></TopDesign>
    <StatisticalNumber></StatisticalNumber>
    <Testimonial></Testimonial>
  </PageLayout>
</template>
