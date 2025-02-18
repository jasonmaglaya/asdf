export default function TeamAvatar({ color, children }) {
  return (
    <div
      className="rounded-circle d-flex align-items-center justify-content-center"
      style={{
        width: "35px",
        height: "35px",
        backgroundColor: "rgba(255, 255, 255, 0.1)",
        flexShrink: 0,
        margin: "2px",
      }}
    >
      <div
        className="rounded-circle d-flex align-items-center justify-content-center text-light"
        style={{
          width: "30px",
          height: "30px",
          backgroundColor: color,
          flexShrink: 0,
        }}
      >
        {children}
      </div>
    </div>
  );
}
