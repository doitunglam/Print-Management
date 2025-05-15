import { onMounted, onBeforeUnmount } from 'vue'
import type { Ref } from 'vue'
export default function useClickOutside(
  component: Ref<string>,
  callback: Function,
  excludeComponent: Ref<string>,
) {
  if (!component) {
    throw new Error('A target component has to be provided.')
  }

  if (!callback) {
    throw new Error('A callback has to be provided.')
  }

  const listener = (event: Event) => {
    if (
      (event.target as string | unknown) === component.value ||
      (event.composedPath() as string[] | [unknown]).includes(
        component.value,
      ) ||
      (event.target as string | unknown) === excludeComponent.value ||
      (event.composedPath() as string[] | [unknown]).includes(
        excludeComponent.value,
      )
    ) {
      return
    }
    if (typeof callback === 'function') {
      callback()
    }
  }

  onMounted(() => {
    window.addEventListener('click', listener)
  })

  onBeforeUnmount(() => {
    window.removeEventListener('click', listener)
  })
}
