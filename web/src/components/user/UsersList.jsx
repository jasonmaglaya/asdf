import { Card, Row, Col, InputGroup, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { StyledCard } from "../_shared/StyledCard";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faUser,
  faSquare,
  faCheckSquare,
} from "@fortawesome/free-solid-svg-icons";
import { Roles } from "../../constants";
import { useEffect, useState } from "react";
import useDebounce from "../../hooks/UseDebounce";

export default function UsersList({
  users,
  handleUserClick,
  handleSearchTextChange,
  isSearching,
  currency,
  locale,
}) {
  const [searchResult, setSearchResult] = useState(users);
  const [keyword, setKeyword] = useState("");
  const navigate = useNavigate();

  useDebounce(
    () => {
      if (handleSearchTextChange) {
        handleSearchTextChange(keyword);
        return;
      }

      if (!keyword?.trim().length) {
        setSearchResult(users);
        return;
      }

      setSearchResult([
        ...users.filter((x) =>
          x.username.toLowerCase().includes(keyword?.toLowerCase())
        ),
      ]);
    },
    [keyword, users, handleSearchTextChange],
    500
  );

  const onSearchTextChange = (e) => setKeyword(e.target.value);

  const onUserClick = (user) => {
    if (!handleUserClick) {
      navigate(`/users/${user?.id}`);
      return;
    }

    handleUserClick(user);
  };

  useEffect(() => {
    setSearchResult(users);
  }, [users]);

  return (
    <>
      <InputGroup className="p-1">
        <input
          type="text"
          className="form-control form-control-lg"
          autoCapitalize="none"
          onChange={onSearchTextChange}
        />
        <Button variant="secondary">SEARCH</Button>
      </InputGroup>
      <div className="d-flex justify-content-between align-items-center p-2">
        <span>Username</span>
        <span>Active</span>
      </div>
      {isSearching ? (
        <div className="d-flex justify-content-center">
          <div className="spinner-border text-info" role="status">
            <span className="visually-hidden">Searching...</span>
          </div>
        </div>
      ) : (
        <></>
      )}
      {searchResult?.map((user) => {
        return (
          <StyledCard
            bg="dark"
            text="white"
            className="m-1"
            key={user.id}
            onClick={() => onUserClick(user)}
            role="button"
          >
            <Card.Body className="p-2">
              <Row>
                <Col className="d-flex align-items-center">
                  <span className="text-nowrap">
                    <i className="text-info p-2">
                      <FontAwesomeIcon icon={faUser} />
                    </i>
                    {user.username}
                  </span>
                </Col>
                <Col className="d-flex align-items-center">
                  <span className="d-none d-sm-block">{Roles[user.role]}</span>
                </Col>
                <Col className="d-flex justify-content-end align-items-center">
                  <span className="text-warning d-none d-sm-block">
                    {user.credits?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </span>
                </Col>
                <Col className="d-flex justify-content-end align-items-center">
                  {user.isActive ? (
                    <i className="text-info p-2">
                      <FontAwesomeIcon icon={faCheckSquare} />
                    </i>
                  ) : (
                    <i className="text-secondary p-2">
                      <FontAwesomeIcon icon={faSquare} />
                    </i>
                  )}
                </Col>
              </Row>
            </Card.Body>
          </StyledCard>
        );
      })}
    </>
  );
}
