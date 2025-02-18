import { Col, Row, Card, Image } from "react-bootstrap";
import logo from "../../branding/logo2.png";

export default function AuthContainer({ children, logoHeight }) {
  const logoH = logoHeight || 240;
  return (
    <Row className="d-flex justify-content-center align-items-center">
      <Col md={8} lg={6} xs={12}>
        <Card bg="dark" text="white">
          <Card.Body>
            <div className="mb-3">
              <div style={{ textAlign: "center", height: `${logoH}px` }}>
                <Image src={logo} style={{ height: "100%" }} />
              </div>
              {children}
            </div>
          </Card.Body>
        </Card>
      </Col>
    </Row>
  );
}
