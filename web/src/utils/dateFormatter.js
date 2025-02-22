const formatUTCToLocalDate = (dateString) => {
  const date = new Date(dateString); // Parse UTC date

  const now = new Date();

  if (now.getFullYear() !== date.getFullYear()) {
    return date
      .toLocaleDateString("en-US", {
        month: "short",
        day: "numeric",
        year: "numeric",
      })
      .toUpperCase();
  }

  return date
    .toLocaleDateString("en-US", { month: "short", day: "numeric" })
    .toUpperCase();
};

function formatUTCToLocalTime(dateString) {
  const date = new Date(dateString); // Parse UTC date
  return date.toLocaleTimeString("en-US", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

export { formatUTCToLocalDate, formatUTCToLocalTime };
