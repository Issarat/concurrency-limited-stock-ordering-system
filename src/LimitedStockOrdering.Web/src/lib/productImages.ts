const productImages = import.meta.glob('../assets/image/*', {
  eager: true,
  import: 'default',
  query: '?url',
}) as Record<string, string>

export function resolveProductImage(imageUrl: string | null) {
  if (!imageUrl) return null

  const filename = imageUrl.split('/').pop()
  const imageEntry = Object.entries(productImages).find(([path]) =>
    path.endsWith(`/${filename}`),
  )

  return imageEntry?.[1] ?? imageUrl
}
