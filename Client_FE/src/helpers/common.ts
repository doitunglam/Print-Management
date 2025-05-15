export const formatMoney = (
  amount: number,
  locale: string = "vi-VN",
  currency: string = "VND"
): string => {
  return new Intl.NumberFormat(locale, {
    style: "currency",
    currency: currency,
  }).format(amount)
}

export const formatNumber = (
  amount: number,
  locales: string = "en-US",
  options?: Intl.NumberFormatOptions
): string => {
  return new Intl.NumberFormat(locales, options).format(amount)
}

export const debounce = <F extends (...args: any[]) => any>(
  func: F,
  delay: number
) => {
  let timeout: ReturnType<typeof setTimeout>
  return function (this: any, ...args: Parameters<F>) {
    clearTimeout(timeout)
    timeout = setTimeout(() => func.apply(this, args), delay)
  }
}
