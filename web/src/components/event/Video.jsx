import { Card } from "react-bootstrap";
import VideoPlayer from "./VideoPlayer";

export default function Video({ embedTag }) {
  return (
    <Card bg="dark" text="white">
      <Card.Body className="text-center" style={{ padding: 0 }}>
        <VideoPlayer embedTag={embedTag}></VideoPlayer>
      </Card.Body>
    </Card>
  );
}
