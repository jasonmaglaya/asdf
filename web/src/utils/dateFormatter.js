const formatUTCToLocalDate = (dateString) => {
  const date = new Date(dateString); // Parse UTC date
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
