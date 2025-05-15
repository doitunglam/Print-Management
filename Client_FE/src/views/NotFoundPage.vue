<template>
  <div
    class="flex w-full min-h-screen justify-center items-center flex-col gap-10"
  >
    <img src="@/assets/images/404.svg" alt="" class="w-60 h-auto" />
    <h3 class="text-cyan-500 font-bold text-4xl">404 NOT FOUND</h3>
    <h4 class="text-cyan-950 font-bold text-2xl">
      Whoops! Trang không tồn tại
    </h4>
    <router-link
      to="/"
      type="button"
      class="w-80 py-3 px-4 text-sm tracking-wide rounded-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none text-center"
    >
      Quay về trang chủ
    </router-link>
  </div>
</template>
