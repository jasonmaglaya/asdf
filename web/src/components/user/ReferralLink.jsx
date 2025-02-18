import { Card, Row, Col, Button } from "react-bootstrap";

export default function ReferralLink({ referralCode }) {
  const referralLink = `${window.location.origin}/signup/${referralCode}`;

  const copy = () => {
    navigator.clipboard.writeText(referralLink);
  };

  return (
    <Card bg="dark" text="white" className="m-1">
      <Card.Header className="d-flex align-items-center p-2">
        Sig Up Link
      </Card.Header>
      <Card.Body>
        <Row>
          <Col className="d-flex flex-column justify-content-center align-items-center text-info">
            <span className="h6 mb-2">{referralLink}</span>
            <Button className="btn btn-lg" onClick={copy}>
              COPY
            </Button>
          </Col>
        </Row>
      </Card.Body>
    </Card>
  );
}
